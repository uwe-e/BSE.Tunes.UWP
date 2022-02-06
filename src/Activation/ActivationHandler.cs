using System.Threading.Tasks;

namespace BSE.Tunes.StoreApp.Activation
{
    // For more information on understanding and extending activation flow see
    // https://github.com/Microsoft/WindowsTemplateStudio/blob/release/docs/UWP/activation.md
    internal abstract class ActivationHandler
    {
        public abstract bool CanHandle(object args);
    }

    // Extend this class to implement new ActivationHandlers
    internal abstract class ActivationHandler<T> : ActivationHandler
        where T : class
    {
        public override bool CanHandle(object args)
        {
            // CanHandle checks the args is of type you have configured
            return args is T && CanHandleInternal(args as T);
        }

        // You can override this method to add extra validation on activation args
        // to determine if your ActivationHandler should handle this activation args
        protected virtual bool CanHandleInternal(T args)
        {
            return true;
        }
    }
}
