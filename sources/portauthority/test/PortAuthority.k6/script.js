import http from 'k6/http';
import uuid from './uuid.js'
import { sleep, check } from 'k6';
import { Trend, Rate } from 'k6/metrics';


let createJobErrorRate = new Rate('Create Job errors');
let createJobTrend = new Trend('Create Job');

let getJobErrorRate = new Rate('Get Job errors');
let getJobTrend = new Trend('Get Job');



export const options = {
    stages: [
        { duration: '1m', target: 100 }, // simulate ramp-up of traffic from 1 to 100 users over 1 minute
        { duration: '5m', target: 100 }, // Stay at 100 users for 5 minutes
        { duration: '1m', target: 0 },   // ramp-down to 0 users
    ],
    thresholds: {
        http_req_duration: ['p(99)<100'], // 99% of requests must complete below 100ms
    },
};

const BASE_URL = 'http://proxy/jobs';

export default function() {
    
    let params = {
        headers: { 'Content-Type' : 'application/json' }
    }
    
    // Create a new job
    let jobId = uuid();
    let createJobResp = http.post(`${BASE_URL}/v1/job`,
        JSON.stringify({
            jobId: jobId,
            type: 'job-loadtest',
            namespace: 'com.portauthority.k6'
        }), params);

    // verify the job payload
    // accepted jobs return a HATEOAS style link ref
    check(createJobResp, {
        'status was 202': (r) => r.status === 202,
        'job create message accepted': (r) => r.json()._links !== null
    }) || createJobErrorRate.add(1);

    createJobTrend.add(createJobResp.timings.duration);

    sleep(1);

    // Fetch the job 
    let getJobResp = http.get(`${BASE_URL}/v1/job/${jobId}`, params)

    // verify the job payload
    // should return the JSON job model
    check(getJobResp, {
        'status was 200': (r) => r.status === 200,
        'job matches requested uuid': (r) => r.json('job').jobId === jobId
    }) || getJobErrorRate.add(1);

    getJobTrend.add(getJobResp.timings.duration);
}