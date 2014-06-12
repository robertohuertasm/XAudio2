using System;
using Windows.Foundation;

namespace Codecoding.XAudio2.Store
{
    public static class MvxPseudoAsyncExtensionMethods
    {
        public static void Await(this IAsyncAction operation)
        {
            try
            {
                var task = operation.AsTask();
                task.Wait();
            }
            catch (AggregateException exception)
            {
                // this possibly oversimplifies the problem report
                throw exception.InnerException;
            }
        }

        public static TResult Await<TResult>(this IAsyncOperation<TResult> operation)
        {
            try
            {
                var task = operation.AsTask();
                task.Wait();
                return task.Result;
            }
            catch (AggregateException exception)
            {
                // this possibly oversimplifies the problem report
                throw exception.InnerException;
            }
        }
    }
}