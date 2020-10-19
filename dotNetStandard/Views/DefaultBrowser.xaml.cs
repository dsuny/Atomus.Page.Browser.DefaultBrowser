using Atomus.Page.Browser.Controllers;
using Atomus.Page.Browser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace Atomus.Page.Browser
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DefaultBrowser : MasterDetailPage, IAction
    {
        private AtomusPageEventHandler beforeActionEventHandler;
        private AtomusPageEventHandler afterActionEventHandler;

        private List<ToolbarItem> WebControlToolbarItem;

        Xamarin.Forms.Page home;

        #region Init
        public DefaultBrowser()
        {
            NavigationPage navigation;
            ICore core;
            string tmp;
            string[] tmps;

            InitializeComponent();

            this.IsPresentedChanged += DefaultBrowserStandard_IsPresentedChanged;


            Master = (Xamarin.Forms.Page)this.CreateInstance("Menu");
            //Master = new Menu.DefaultMenu();

            ((IAction)Master).BeforeActionEventHandler += MasterPage_BeforeActionEventHandler;
            ((IAction)Master).AfterActionEventHandler += MasterPage_AfterActionEventHandler;

            navigation = new NavigationPage();
            navigation.Pushed += Navigation_Pushed;
            navigation.Popped += Navigation_Popped;
            navigation.Appearing += Navigation_Appearing;
            navigation.BarBackgroundColor = ((string)Config.Client.GetAttribute("BarBackgroundColor")).ToColor();

            Detail = navigation;

            tmp = this.GetAttribute("HomeType");

            if (tmp.IsNullOrEmpty())
            {
                this.CreateControlToolbarItem(true);

                core = this.CreateInstance("Home");
                //core = new Home.DefaultHomeStandard();
            }
            else
            {
                this.CreateControlToolbarItem(false);

                tmps = tmp.Split(',');
                core = this.OpenControl(tmps[0].ToDecimal(), tmps[1].ToDecimal(), null, null, true);
                core.SetAttribute("PageType", "Page");
            }
            ((IAction)core).BeforeActionEventHandler += Home_BeforeActionEventHandler;
            ((IAction)core).AfterActionEventHandler += Home_AfterActionEventHandler;

            home = core as Xamarin.Forms.Page;

            navigation.PushAsync(home, false);
        }


        private void Home_BeforeActionEventHandler(ICore sender, AtomusPageEventArgs e) { }
        private void Home_AfterActionEventHandler(ICore sender, AtomusPageEventArgs e) { }

        private void DefaultBrowserStandard_IsPresentedChanged(object sender, EventArgs e)
        {
            ((IAction)Master).ControlAction(this, "Menu.LoadInfo", null);
        }

        private void MasterPage_BeforeActionEventHandler(ICore sender, AtomusPageEventArgs e) { }
        private void MasterPage_AfterActionEventHandler(ICore sender, AtomusPageEventArgs e)
        {
            ICore core;
            object[] objects;
            Xamarin.Forms.Page page;

            switch (e.Action)
            {
                case "Menu.Select":
                    core = (ICore)e.Value;

                    page = (Xamarin.Forms.Page)e.Value;

                    var existingPages = ((NavigationPage)Detail).Pages.ToList();

                    if (existingPages.Contains(page))
                    {
                        for (int i = existingPages.Count - 1; i >= 0; i--)
                        {
                            if (existingPages[i].Equals(page))
                            {
                                break;
                            }
                            else
                                ((NavigationPage)Detail).PopAsync();
                        }
                    }
                    else
                    {
                        ((NavigationPage)Detail).PushAsync((Xamarin.Forms.Page)e.Value, false);
                    }

                    IsPresented = false;
                    break;

                case "Menu.OpenControl":
                    objects = (object[])e.Value;//_MENU_ID, _ASSEMBLY_ID, _VisibleOne

                    if ((bool)objects[2])//_VisibleOne
                    {
                        //foreach (TabPage _TabPage in this.tabControl.TabPages)
                        //{
                        //    core = (ICore)_TabPage.Tag;

                        //    MENU_ID = core.GetAttribute("MENU_ID");

                        //    if (MENU_ID != null)
                        //        if (MENU_ID.Equals(objects[0].ToString()))
                        //        {
                        //            this.tabControl.Tag = _TabPage.Tag;
                        //            this.tabControl.SelectedTab = _TabPage;
                        //            return;//기존 화면이 있으니 바로 빠져 나감
                        //        }
                        //}
                    }

                    e.Value = this.OpenControl((decimal)objects[0], (decimal)objects[1], sender, null, true);

                    break;
            }
        }
#endregion

        #region IO
        object IAction.ControlAction(ICore sender, AtomusPageArgs e)
        {
            try
            {
                this.beforeActionEventHandler?.Invoke(this, e);

                //switch (e.Action)
                //{
                //    case "New":
                //        return this.InitControl();

                //    case "Search":
                //        if (e.Value != null && e.Value is DataTable && (e.Value as DataTable).Rows.Count > 0)
                //            return this.Search(e.Value as DataTable);
                //        else
                //            return this.Search();

                //    case "Button1":
                //        return this.SearchBest();

                //    case "Button2":
                //        return this.ShowAdd();

                //    case "Button3":
                //        return this.ShowTrend();

                //    default:
                //        throw new AtomusException("'{0}'은 처리할 수 없는 Action 입니다.".Translate(e.Action));
                //}

                return true;
            }
            finally
            {
                this.afterActionEventHandler?.Invoke(this, e);
            }
        }

        private ICore OpenControl(decimal _MENU_ID, decimal _ASSEMBLY_ID, ICore sender, AtomusPageEventArgs _AtomusControlEventArgs, bool addTabControl)
        {
            Service.IResponse _Result;
            IAction _Core;

            try
            {
                _Result = this.SearchOpenControl(new DefaultBrowserSearchModel()
                {
                    MENU_ID = _MENU_ID,
                    ASSEMBLY_ID = _ASSEMBLY_ID
                });

                if (_Result.Status == Service.Status.OK)
                {
                    if (_Result.DataSet.Tables.Count == 2)
                        if (_Result.DataSet.Tables[0].Rows.Count == 1)
                        {
                            //switch (_ASSEMBLY_ID)
                            //{
                            //    //case 5:
                            //    //    _Core = new Atomus.AutoKing.Page.AccountStandard();//  ▶ 계정
                            //    //    break;

                            //    //case 12:
                            //    //    _Core = new Atomus.AutoKing.Page.CM02();//▶ 수익 & 랭킹
                            //    //    break;

                            //    //case 15:
                            //    //    _Core = new Atomus.AutoKing.Page.CM05();//▶ 자산 변동
                            //    //    break;

                            //    //case 11:
                            //    //    _Core = new Atomus.AutoKing.Page.CM03();//  ▶ 스켈핑(서버)
                            //    //    break;

                            //    //case 13:
                            //    //    _Core = new Atomus.AutoKing.Page.CM04();//  ▶ 포인트 관리
                            //    //    break;

                            //    //case 14:
                            //    //    _Core = new Atomus.AutoKing.Page.CM11();//  ▶ 환경 설정
                            //    //    break;

                            //    //case 16:
                            //    //    _Core = new Atomus.AutoKing.Page.CM15();//  ▶ AutoKing Info
                            //    //    break;

                            //    //case 9:
                            //    //    _Core = new Atomus.AutoKing.Page.CM21();//  ▶ 입금, 출금 조회
                            //    //    break;

                            //    //case 7:
                            //    //    _Core = new Atomus.AutoKing.Page.CM22();//  ▶ 자동매매 서버(스켈핑) 관리
                            //    //    break;

                            //    default:
                            //        if (_Result.DataSet.Tables[0].Columns.Contains("FILE_TEXT") && _Result.DataSet.Tables[0].Rows[0]["FILE_TEXT"] != DBNull.Value)
                            //            _Core = (IAction)Factory.CreateInstance(Convert.FromBase64String((string)_Result.DataSet.Tables[0].Rows[0]["FILE_TEXT"]), _Result.DataSet.Tables[0].Rows[0]["NAMESPACE"].ToString(), false, false);
                            //        else
                            //            _Core = (IAction)Factory.CreateInstance((byte[])_Result.DataSet.Tables[0].Rows[0]["FILE"], _Result.DataSet.Tables[0].Rows[0]["NAMESPACE"].ToString(), false, false);

                            //        break;
                            //}

                            _Core = (IAction)Config.Client.GetAttribute("DebugPage");
                            if (_Core != null && _Core.GetAttributeDecimal("ASSEMBLY_ID") == _ASSEMBLY_ID) { }
                            else
                            {
                                if (_Result.DataSet.Tables[0].Columns.Contains("FILE_TEXT") && _Result.DataSet.Tables[0].Rows[0]["FILE_TEXT"] != DBNull.Value)
                                    _Core = (IAction)Factory.CreateInstance(Convert.FromBase64String((string)_Result.DataSet.Tables[0].Rows[0]["FILE_TEXT"]), _Result.DataSet.Tables[0].Rows[0]["NAMESPACE"].ToString(), false, false);
                                else
                                    _Core = (IAction)Factory.CreateInstance((byte[])_Result.DataSet.Tables[0].Rows[0]["FILE"], _Result.DataSet.Tables[0].Rows[0]["NAMESPACE"].ToString(), false, false);

                                _Core.SetAttribute("MENU_ID", _MENU_ID.ToString());
                                _Core.SetAttribute("ASSEMBLY_ID", _ASSEMBLY_ID.ToString());
                            } 

                            _Core.BeforeActionEventHandler += UserControl_BeforeActionEventHandler;
                            _Core.AfterActionEventHandler += UserControl_AfterActionEventHandler;

                            foreach (System.Data.DataRow _DataRow in _Result.DataSet.Tables[1].Rows)
                            {
                                _Core.SetAttribute(_DataRow["ATTRIBUTE_NAME"].ToString(), _DataRow["ATTRIBUTE_VALUE"].ToString());
                            }

                            if (_AtomusControlEventArgs != null)
                                _Core.ControlAction(sender, _AtomusControlEventArgs.Action, _AtomusControlEventArgs.Value);

                            return _Core;
                        }
                }
                else
                {
                    DisplayAlert("Warning", _Result.Message, "OK");
                }

                return null;
            }
            catch (Exception _Exception)
            {
                DisplayAlert("Exception", _Exception.Message, "OK");
                return null;
            }
            finally
            {
            }
        }
        #endregion

        #region Event
        event AtomusPageEventHandler IAction.BeforeActionEventHandler
        {
            add
            {
                this.beforeActionEventHandler += value;
            }
            remove
            {
                this.beforeActionEventHandler -= value;
            }
        }
        event AtomusPageEventHandler IAction.AfterActionEventHandler
        {
            add
            {
                this.afterActionEventHandler += value;
            }
            remove
            {
                this.afterActionEventHandler -= value;
            }
        }

        private void UserControl_BeforeActionEventHandler(ICore sender, AtomusPageEventArgs e) { }
        private async void UserControl_AfterActionEventHandler(ICore sender, AtomusPageEventArgs e)
        {
            object[] objects;

            try
            {
                switch (e.Action)
                {
                    case "UserControl.OpenControl":
                        objects = (object[])e.Value;//_MENU_ID, _ASSEMBLY_ID, sender, AtomusControlArgs

                        e.Value = this.OpenControl((decimal)objects[0], (decimal)objects[1], sender, (AtomusPageEventArgs)objects[2], true);

                        break;

                    case "UserControl.GetControl":
                        objects = (object[])e.Value;//_MENU_ID, _ASSEMBLY_ID, sender, AtomusControlArgs

                        e.Value = this.OpenControl((decimal)objects[0], (decimal)objects[1], sender, (AtomusPageEventArgs)objects[2], false);

                        break;

                        //case "UserControl.AssemblyVersionCheck":
                        //    tmp = this.GetAttribute("ProcedureAssemblyVersionCheck");

                        //    if (tmp != null && tmp.Trim() != "")
                        //    {
                        //        response = this.AssemblyVersionCheck(sender);

                        //        if (response.Status != Service.Status.OK)
                        //        {
                        //            this.MessageBoxShow(this, response.Message);
                        //            e.Value = false;
                        //        }
                        //        else
                        //            e.Value = true;
                        //    }
                        //    else
                        //        e.Value = true;

                        //    break;

                        //default:
                        //    throw new AtomusException("'{0}'은 처리할 수 없는 Action 입니다.".Translate(e.Action));
                }
            }
            catch (Exception exception)
            {
                await DisplayAlert("Exception", exception.Message, "OK");
            }
        }

        private void Navigation_Popped(object sender, NavigationEventArgs e)
        {
            this.AddWebViewToolbarItem();
        }
        private void Navigation_Pushed(object sender, NavigationEventArgs e)
        {
            this.AddWebViewToolbarItem();
        }
        private void Navigation_Appearing(object sender, EventArgs e)
        {
            this.AddWebViewToolbarItem();
        }

        private void ToolbarItemHome_Clicked(object sender, EventArgs e)
        {
            IAction action;

            action = (IAction)((NavigationPage)Detail).CurrentPage;

            if (action.GetAttribute("PageType") != null && action.GetAttribute("PageType") == "WebView")
                action.ControlAction(this, new AtomusPageArgs("WebVIew.Home", null));
        }
        private void ToolbarItemBack_Clicked(object sender, EventArgs e)
        {
            IAction action;

            action = (IAction)((NavigationPage)Detail).CurrentPage;

            if (action.GetAttribute("PageType") != null && action.GetAttribute("PageType") == "WebView")
                action.ControlAction(this, new AtomusPageArgs("WebVIew.Back", null));
        }
        private void ToolbarItemForward_Clicked(object sender, EventArgs e)
        {
            IAction action;

            action = (IAction)((NavigationPage)Detail).CurrentPage;

            if (action.GetAttribute("PageType") != null && action.GetAttribute("PageType") == "WebView")
                action.ControlAction(this, new AtomusPageArgs("WebVIew.Forward", null));
        }
        private async void ToolbarItemExit_Clicked(object sender, EventArgs e)
        {
            var answer = await DisplayAlert("Exit", "Do you wan't to exit the App?", "Yes", "No");

            if (answer)
                DependencyService.Get<Atomus.Page.INativeHelper>().CloseApp();
        }

        protected override bool OnBackButtonPressed()
        {
            if (((NavigationPage)Detail).CurrentPage == ((NavigationPage)Detail).RootPage)
                this.ToolbarItemBack_Clicked(null, null);
            else
                ((NavigationPage)Detail).PopAsync();

            return true;
        }
        #endregion


        #region Etc
        private void AddWebViewToolbarItem()
        {
            this.AddWebViewToolbarItem((ICore)((NavigationPage)Detail).CurrentPage);
        }
        private void AddWebViewToolbarItem(ICore core)
        {
            if (core.GetAttribute("PageType") != null && new string[] { "WebView", "Page" }.Contains(core.GetAttribute("PageType")))
            {
                if (((Xamarin.Forms.Page)core).ToolbarItems.Count > 0)
                    (from sel in this.WebControlToolbarItem
                     join sel1 in ((Xamarin.Forms.Page)core).ToolbarItems on sel.Text equals sel1.Text
                     where !sel.Equals(sel1)
                     select sel).ForEach(x => ((Xamarin.Forms.Page)core).ToolbarItems.Add(x));
                else
                    (from sel in this.WebControlToolbarItem
                     select sel).ForEach(x => ((Xamarin.Forms.Page)core).ToolbarItems.Add(x));
            }
            else
            {
                if (((Xamarin.Forms.Page)core).ToolbarItems.Count > 0)
                    ((Xamarin.Forms.Page)core).ToolbarItems.Clear();
            }
        }

        private void CreateControlToolbarItem(bool isWebControl)
        {
            ToolbarItem toolbarItem;

            this.WebControlToolbarItem = new List<ToolbarItem>();

            if (isWebControl)
            {
                toolbarItem = new ToolbarItem() { Text = "H" };
                toolbarItem.Priority = 0;
                toolbarItem.Order = ToolbarItemOrder.Primary;
                toolbarItem.Clicked += ToolbarItemHome_Clicked;
                this.WebControlToolbarItem.Add(toolbarItem);

                toolbarItem = new ToolbarItem() { Text = "<" };
                toolbarItem.Priority = 0;
                toolbarItem.Order = ToolbarItemOrder.Primary;
                toolbarItem.Clicked += ToolbarItemBack_Clicked;
                this.WebControlToolbarItem.Add(toolbarItem);

                toolbarItem = new ToolbarItem() { Text = ">" };
                toolbarItem.Priority = 0;
                toolbarItem.Order = ToolbarItemOrder.Primary;
                toolbarItem.Clicked += ToolbarItemForward_Clicked;
                this.WebControlToolbarItem.Add(toolbarItem);
            }

            toolbarItem = new ToolbarItem() { Text = "X" };
            toolbarItem.Priority = 0;
            toolbarItem.Order = ToolbarItemOrder.Primary;
            toolbarItem.Clicked += ToolbarItemExit_Clicked;
            this.WebControlToolbarItem.Add(toolbarItem);
        }
#endregion
    }
}