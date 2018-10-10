using Framework;

using Sandbox.GraphQL;

namespace Sandbox.RGC
{
    public class FGCAccount : IAccountProvider, IBroker
    {
        #region IFGCProvider

        public void Initialize()
        {
            //this.Publish(new OnHandleGraphRequestSignal());
        }

        public void LoginAsGuest()
        {

        }

        public void LoginAsFB()
        {

        }

        public void FetchData()
        {

        }

        #endregion
    }
}
