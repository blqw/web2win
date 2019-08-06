using CefSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace web2win.Plugins
{
    class MenuPlugin : PluginBase
    {
        public bool EnabledHistory { get; private set; }

        public override void Configuration(Config config)
        {
            Enabled = config.DisableRightClick;
            EnabledHistory = !config.DisableHistory;
        }

        public void OnBeforeContextMenu(PluginEventArgs args)
        {
            var model = args.Get<IMenuModel>();
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
                    case CefMenuCommand.Back:
                    case CefMenuCommand.Forward:
                        if (EnabledHistory)
                        {
                            flag = true;
                            continue;
                        }
                        break;
                    default:
                        break;
                }
                model.RemoveAt(i);
            }
        }
    }
}
