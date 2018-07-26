using System.Collections;

using uPromise;

using Framework;

namespace Sandbox.Popup
{
    public interface IPopupWindow
    {
        Promise In();
        Promise Out();
    }
}