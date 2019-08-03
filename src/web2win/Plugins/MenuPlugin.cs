using CefSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace web2win.Plugins
{
    class MenuPlugin : PluginBase, IContextMenuHandler
    {
        public override void Configuration(Config config) => Enabled = config.DisableRightClick;

        void IContextMenuHandler.OnBeforeContextMenu(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame,
            IContextMenuParams parameters, IMenuModel model)
        {
            var flag = false;
            for (var i = model.Count - 1; i >= 0; i--)
            {
                switch (model.GetCommandIdAt(i))
                {
                    case CefMenuCommand.Undo:
                    case CefMenuCommand.Redo:
                    case CefMenuCommand.Cut:
                    case CefMenuCommand.Copy:
                    case CefMenuCommand.Paste:
                    case CefMenuCommand.Delete:
                    case CefMenuCommand.SelectAll:
                    case CefMenuCommand.Find:
                        flag = true;
                        continue;
                    case CefMenuCommand.NotFound:
                        switch (model.GetTypeAt(i))
                        {
                            case MenuItemType.Separator:
                                if (flag && i != 0)
                                {
                                    continue;
                                }
                                flag = false;
                                break;
                            case MenuItemType.None:
                            case MenuItemType.Command:
                            case MenuItemType.Check:
                            case MenuItemType.Radio:
                            case MenuItemType.SubMenu:
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
                model.RemoveAt(i);
            }
        }

        bool IContextMenuHandler.OnContextMenuCommand(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame,
            IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags)
            => false;
        void IContextMenuHandler.OnContextMenuDismissed(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame) { }

        bool IContextMenuHandler.RunContextMenu(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame,
            IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback)
            => false;
    }
}
