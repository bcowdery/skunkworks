import http from 'k6/http';
import uuid from './uuid.js'
import { sleep, check, group, fail } from 'k6';
import { Trend, Rate } from 'k6/metrics';


let createJobErrorRate = new Rate('Create Job errors');
let createJobTrend = new Trend('Create Job');

let getJobErrorRate = new Rate('Get Job errors');
let getJobTrend = new Trend('Get Job');

let updateJobErrorRate = new Rate('Update Job errors');
let updateJobTrend = new Trend('Update Job');


/**
 * Runtime options
 */
export const options = {
    stages: [
        { duration: '5m',  target: 250 }, // simulate ramp-up of traffic from 1 to 250 users over 5 minutes
        { duration: '10m', target: 250 }, // Stay at 250 users for 10 minutes
        { duration: '1m',  target: 0 },   // ramp-down to 0 users
    ],
    thresholds: {
        'Create Job errors': ['rate<0.1'],  // 1% of total create job requests are allowed to fail
        'Get Job errors':    ['rate<0.1'],  // 1% of total get job requests are allowed to fail
        'Update Job errors': ['rate<0.1'],  // 1% of total update job requests are allowd to fail
        http_req_duration:   ['p(99)<100'], // 99% of requests must complete in less than 100ms
    },
};

/**
 * Base URL for the PortAuthority job endpoint. 
 * 
 * This should be pointed at the Nginx proxy sitting infront of the services. The k6 container 
 * will need to on the same docker network as the skunkworks application.
 * 
 * @type string
 */
const BASE_URL = 'http://proxy/jobs';


/**
 * Virtual User test
 */
export default function() {
    
    const requestConfigWithTag = tag => ({
        headers: {
            'Content-Type': 'application/json'
        },
        tags: Object.assign({}, {
            name: 'PortAuthority'
        }, tag)
    });

    group('Create and modify jobs', () => {
       
        let jobId = uuid();
                
        /**
         * POST a job creation message and verify that it was accepted.
         */
        group('Create job', () => {

            const payload = JSON.stringify({
                jobId: jobId,
                type: 'job-loadtest',
                namespace: 'com.portauthority.k6'                
            });

            const res = http.post(`${BASE_URL}/v1/job`, payload, requestConfigWithTag({ name: 'Create' }));

            createJobTrend.add(res.timings.duration);

            check(res, {
                'status was 202': (r) => r.status === 202,
                'job create message accepted': (r) => r.json()._links !== null
            }) || createJobErrorRate.add(1);
        });

        /**
         * GET the created job and verify the returned payload.
         */
        group('Get job', () => {
                        
            sleep(5); // wait for async create message to complete

            const res = http.get(`${BASE_URL}/v1/job/${jobId}`, requestConfigWithTag({ name: 'Get' }));

            getJobTrend.add(res.timings.duration);

            if (!check(res, { 'job created correctly': (r) => r.status === 200 })) {         
                console.log(`Unable to create job ${res.status}, ${res.body}`)
                getJobErrorRate.add(1);
                return;
            }            
        });
        
        /**
         * PUT start the job and update the status to 'InProgress'
         */
        group('Start job', () => {

            const payload = JSON.stringify({
                startTime: new Date().toISOString()             
            });

            const res = http.put(`${BASE_URL}/v1/job/${jobId}/start`, payload, requestConfigWithTag({ name: 'Start' }));
            
            updateJobTrend.add(res.timings.duration);

            check(res, {
                'status was 202': (r) => r.status === 202,
            }) || updateJobErrorRate.add(1);
        });


        /**
         * PUT end the job and update the status to 'Completed'
         */
        group('End job', () => {

            sleep(5); // wait for async start message to complete
                      // simulate some time that it would take an actual background job to do work and report in

            const payload = JSON.stringify({
                success: true,
                endTime: new Date().toISOString()             
            });

            const res = http.put(`${BASE_URL}/v1/job/${jobId}/end`, payload, requestConfigWithTag({ name: 'End' }));
            
            updateJobTrend.add(res.timings.duration);

            check(res, {
                'status was 202': (r) => r.status === 202,
            }) || updateJobErrorRate.add(1);
        });        
    });
}