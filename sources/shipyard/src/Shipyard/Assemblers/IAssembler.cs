namespace Shipyard.Assemblers
{
    public interface IAssembler<in TArg, out TModel>
    {
        TModel Assemble(TArg arg);
    }

    public interface IAssembler<in TArg, in TArg1, out TModel>
    {
        TModel Assemble(TArg arg, TArg1 arg1);
    }
    
    public interface IAssembler<in TArg, in TArg1, in TArg2, out TModel>
    {
        TModel Assemble(TArg arg, TArg1 arg1, TArg2 arg2);
    }
}
