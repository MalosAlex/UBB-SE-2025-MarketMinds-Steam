﻿#pragma checksum "C:\Users\Bianca\Desktop\ISS\UBB-SE-2025-Spice\SteamProfile\Views\ConfigurationsView\ModifyProfilePage.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "77B31A2F7629588C6A8E6A7C1EA7AE3A7D15E671E344326A54D956B8B7B0D211"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SteamProfile.Views.ConfigurationsView
{
    partial class ModifyProfilePage : 
        global::Microsoft.UI.Xaml.Controls.Page, 
        global::Microsoft.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.UI.Xaml.Markup.Compiler"," 3.0.0.2312")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        private static class XamlBindingSetters
        {
            public static void Set_Microsoft_UI_Xaml_Controls_TextBlock_Text(global::Microsoft.UI.Xaml.Controls.TextBlock obj, global::System.String value, string targetNullValue)
            {
                if (value == null && targetNullValue != null)
                {
                    value = targetNullValue;
                }
                obj.Text = value ?? global::System.String.Empty;
            }
            public static void Set_Microsoft_UI_Xaml_UIElement_Visibility(global::Microsoft.UI.Xaml.UIElement obj, global::Microsoft.UI.Xaml.Visibility value)
            {
                obj.Visibility = value;
            }
            public static void Set_Microsoft_UI_Xaml_Controls_TextBox_Text(global::Microsoft.UI.Xaml.Controls.TextBox obj, global::System.String value, string targetNullValue)
            {
                if (value == null && targetNullValue != null)
                {
                    value = targetNullValue;
                }
                obj.Text = value ?? global::System.String.Empty;
            }
            public static void Set_Microsoft_UI_Xaml_Controls_Primitives_ButtonBase_Command(global::Microsoft.UI.Xaml.Controls.Primitives.ButtonBase obj, global::System.Windows.Input.ICommand value, string targetNullValue)
            {
                if (value == null && targetNullValue != null)
                {
                    value = (global::System.Windows.Input.ICommand) global::Microsoft.UI.Xaml.Markup.XamlBindingHelper.ConvertValue(typeof(global::System.Windows.Input.ICommand), targetNullValue);
                }
                obj.Command = value;
            }
            public static void Set_Microsoft_UI_Xaml_Controls_Control_IsEnabled(global::Microsoft.UI.Xaml.Controls.Control obj, global::System.Boolean value)
            {
                obj.IsEnabled = value;
            }
        };

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.UI.Xaml.Markup.Compiler"," 3.0.0.2312")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        private class ModifyProfilePage_obj1_Bindings :
            global::Microsoft.UI.Xaml.Markup.IDataTemplateComponent,
            global::Microsoft.UI.Xaml.Markup.IXamlBindScopeDiagnostics,
            global::Microsoft.UI.Xaml.Markup.IComponentConnector,
            IModifyProfilePage_Bindings
        {
            private global::SteamProfile.Views.ConfigurationsView.ModifyProfilePage dataRoot;
            private bool initialized = false;
            private const int NOT_PHASED = (1 << 31);
            private const int DATA_CHANGED = (1 << 30);

            // Fields for each control that has bindings.
            private global::Microsoft.UI.Xaml.Controls.TextBlock obj2;
            private global::Microsoft.UI.Xaml.Controls.TextBox obj3;
            private global::Microsoft.UI.Xaml.Controls.TextBlock obj4;
            private global::Microsoft.UI.Xaml.Controls.Button obj5;
            private global::Microsoft.UI.Xaml.Controls.Button obj7;
            private global::Microsoft.UI.Xaml.Controls.TextBlock obj8;

            // Static fields for each binding's enabled/disabled state
            private static bool isobj2TextDisabled = false;
            private static bool isobj2VisibilityDisabled = false;
            private static bool isobj3TextDisabled = false;
            private static bool isobj4TextDisabled = false;
            private static bool isobj4VisibilityDisabled = false;
            private static bool isobj5CommandDisabled = false;
            private static bool isobj5IsEnabledDisabled = false;
            private static bool isobj7CommandDisabled = false;
            private static bool isobj8TextDisabled = false;

            private ModifyProfilePage_obj1_BindingsTracking bindingsTracking;

            public ModifyProfilePage_obj1_Bindings()
            {
                this.bindingsTracking = new ModifyProfilePage_obj1_BindingsTracking(this);
            }

            public void Disable(int lineNumber, int columnNumber)
            {
                if (lineNumber == 31 && columnNumber == 24)
                {
                    isobj2TextDisabled = true;
                }
                else if (lineNumber == 33 && columnNumber == 24)
                {
                    isobj2VisibilityDisabled = true;
                }
                else if (lineNumber == 53 && columnNumber == 22)
                {
                    isobj3TextDisabled = true;
                }
                else if (lineNumber == 56 && columnNumber == 24)
                {
                    isobj4TextDisabled = true;
                }
                else if (lineNumber == 58 && columnNumber == 22)
                {
                    isobj4VisibilityDisabled = true;
                }
                else if (lineNumber == 65 && columnNumber == 25)
                {
                    isobj5CommandDisabled = true;
                }
                else if (lineNumber == 66 && columnNumber == 25)
                {
                    isobj5IsEnabledDisabled = true;
                }
                else if (lineNumber == 39 && columnNumber == 25)
                {
                    isobj7CommandDisabled = true;
                }
                else if (lineNumber == 41 && columnNumber == 28)
                {
                    isobj8TextDisabled = true;
                }
            }

            // IComponentConnector

            public void Connect(int connectionId, global::System.Object target)
            {
                switch(connectionId)
                {
                    case 2: // Views\ConfigurationsView\ModifyProfilePage.xaml line 31
                        this.obj2 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.TextBlock>(target);
                        break;
                    case 3: // Views\ConfigurationsView\ModifyProfilePage.xaml line 49
                        this.obj3 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.TextBox>(target);
                        this.bindingsTracking.RegisterTwoWayListener_3(this.obj3);
                        break;
                    case 4: // Views\ConfigurationsView\ModifyProfilePage.xaml line 56
                        this.obj4 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.TextBlock>(target);
                        break;
                    case 5: // Views\ConfigurationsView\ModifyProfilePage.xaml line 62
                        this.obj5 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                        break;
                    case 7: // Views\ConfigurationsView\ModifyProfilePage.xaml line 38
                        this.obj7 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                        break;
                    case 8: // Views\ConfigurationsView\ModifyProfilePage.xaml line 40
                        this.obj8 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.TextBlock>(target);
                        break;
                    default:
                        break;
                }
            }
                        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.UI.Xaml.Markup.Compiler"," 3.0.0.2312")]
                        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
                        public global::Microsoft.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target) 
                        {
                            return null;
                        }

            // IDataTemplateComponent

            public void ProcessBindings(global::System.Object item, int itemIndex, int phase, out int nextPhase)
            {
                nextPhase = -1;
            }

            public void Recycle()
            {
                return;
            }

            // IModifyProfilePage_Bindings

            public void Initialize()
            {
                if (!this.initialized)
                {
                    this.Update();
                }
            }
            
            public void Update()
            {
                this.Update_(this.dataRoot, NOT_PHASED);
                this.initialized = true;
            }

            public void StopTracking()
            {
                this.bindingsTracking.ReleaseAllListeners();
                this.initialized = false;
            }

            public void DisconnectUnloadedObject(int connectionId)
            {
                throw new global::System.ArgumentException("No unloadable elements to disconnect.");
            }

            public bool SetDataRoot(global::System.Object newDataRoot)
            {
                this.bindingsTracking.ReleaseAllListeners();
                if (newDataRoot != null)
                {
                    this.dataRoot = global::WinRT.CastExtensions.As<global::SteamProfile.Views.ConfigurationsView.ModifyProfilePage>(newDataRoot);
                    return true;
                }
                return false;
            }

            public void Activated(object obj, global::Microsoft.UI.Xaml.WindowActivatedEventArgs data)
            {
                this.Initialize();
            }

            public void Loading(global::Microsoft.UI.Xaml.FrameworkElement src, object data)
            {
                this.Initialize();
            }

            // Update methods for each path node used in binding steps.
            private void Update_(global::SteamProfile.Views.ConfigurationsView.ModifyProfilePage obj, int phase)
            {
                if (obj != null)
                {
                    if ((phase & (NOT_PHASED | DATA_CHANGED | (1 << 0))) != 0)
                    {
                        this.Update_ViewModel(obj.ViewModel, phase);
                    }
                }
            }
            private void Update_ViewModel(global::SteamProfile.ViewModels.ConfigurationsViewModels.ModifyProfileViewModel obj, int phase)
            {
                this.bindingsTracking.UpdateChildListeners_ViewModel(obj);
                if (obj != null)
                {
                    if ((phase & (NOT_PHASED | DATA_CHANGED | (1 << 0))) != 0)
                    {
                        this.Update_ViewModel_SuccessMessage(obj.SuccessMessage, phase);
                        this.Update_ViewModel_SuccessMessageVisibility(obj.SuccessMessageVisibility, phase);
                        this.Update_ViewModel_Description(obj.Description, phase);
                        this.Update_ViewModel_DescriptionErrorMessage(obj.DescriptionErrorMessage, phase);
                        this.Update_ViewModel_DescriptionErrorVisibility(obj.DescriptionErrorVisibility, phase);
                    }
                    if ((phase & (NOT_PHASED | (1 << 0))) != 0)
                    {
                        this.Update_ViewModel_SaveChangesCommand(obj.SaveChangesCommand, phase);
                    }
                    if ((phase & (NOT_PHASED | DATA_CHANGED | (1 << 0))) != 0)
                    {
                        this.Update_ViewModel_CanSave(obj.CanSave, phase);
                    }
                    if ((phase & (NOT_PHASED | (1 << 0))) != 0)
                    {
                        this.Update_ViewModel_ChooseNewPhotoCommand(obj.ChooseNewPhotoCommand, phase);
                    }
                    if ((phase & (NOT_PHASED | DATA_CHANGED | (1 << 0))) != 0)
                    {
                        this.Update_ViewModel_SelectedImageName(obj.SelectedImageName, phase);
                    }
                }
            }
            private void Update_ViewModel_SuccessMessage(global::System.String obj, int phase)
            {
                if ((phase & ((1 << 0) | NOT_PHASED | DATA_CHANGED)) != 0)
                {
                    // Views\ConfigurationsView\ModifyProfilePage.xaml line 31
                    if (!isobj2TextDisabled)
                    {
                        XamlBindingSetters.Set_Microsoft_UI_Xaml_Controls_TextBlock_Text(this.obj2, obj, null);
                    }
                }
            }
            private void Update_ViewModel_SuccessMessageVisibility(global::Microsoft.UI.Xaml.Visibility obj, int phase)
            {
                if ((phase & ((1 << 0) | NOT_PHASED | DATA_CHANGED)) != 0)
                {
                    // Views\ConfigurationsView\ModifyProfilePage.xaml line 31
                    if (!isobj2VisibilityDisabled)
                    {
                        XamlBindingSetters.Set_Microsoft_UI_Xaml_UIElement_Visibility(this.obj2, obj);
                    }
                }
            }
            private void Update_ViewModel_Description(global::System.String obj, int phase)
            {
                if ((phase & ((1 << 0) | NOT_PHASED | DATA_CHANGED)) != 0)
                {
                    // Views\ConfigurationsView\ModifyProfilePage.xaml line 49
                    if (!isobj3TextDisabled)
                    {
                        XamlBindingSetters.Set_Microsoft_UI_Xaml_Controls_TextBox_Text(this.obj3, obj, null);
                    }
                }
            }
            private void Update_ViewModel_DescriptionErrorMessage(global::System.String obj, int phase)
            {
                if ((phase & ((1 << 0) | NOT_PHASED | DATA_CHANGED)) != 0)
                {
                    // Views\ConfigurationsView\ModifyProfilePage.xaml line 56
                    if (!isobj4TextDisabled)
                    {
                        XamlBindingSetters.Set_Microsoft_UI_Xaml_Controls_TextBlock_Text(this.obj4, obj, null);
                    }
                }
            }
            private void Update_ViewModel_DescriptionErrorVisibility(global::Microsoft.UI.Xaml.Visibility obj, int phase)
            {
                if ((phase & ((1 << 0) | NOT_PHASED | DATA_CHANGED)) != 0)
                {
                    // Views\ConfigurationsView\ModifyProfilePage.xaml line 56
                    if (!isobj4VisibilityDisabled)
                    {
                        XamlBindingSetters.Set_Microsoft_UI_Xaml_UIElement_Visibility(this.obj4, obj);
                    }
                }
            }
            private void Update_ViewModel_SaveChangesCommand(global::CommunityToolkit.Mvvm.Input.IAsyncRelayCommand obj, int phase)
            {
                if ((phase & ((1 << 0) | NOT_PHASED )) != 0)
                {
                    // Views\ConfigurationsView\ModifyProfilePage.xaml line 62
                    if (!isobj5CommandDisabled)
                    {
                        XamlBindingSetters.Set_Microsoft_UI_Xaml_Controls_Primitives_ButtonBase_Command(this.obj5, obj, null);
                    }
                }
            }
            private void Update_ViewModel_CanSave(global::System.Boolean obj, int phase)
            {
                if ((phase & ((1 << 0) | NOT_PHASED | DATA_CHANGED)) != 0)
                {
                    // Views\ConfigurationsView\ModifyProfilePage.xaml line 62
                    if (!isobj5IsEnabledDisabled)
                    {
                        XamlBindingSetters.Set_Microsoft_UI_Xaml_Controls_Control_IsEnabled(this.obj5, obj);
                    }
                }
            }
            private void Update_ViewModel_ChooseNewPhotoCommand(global::CommunityToolkit.Mvvm.Input.IAsyncRelayCommand obj, int phase)
            {
                if ((phase & ((1 << 0) | NOT_PHASED )) != 0)
                {
                    // Views\ConfigurationsView\ModifyProfilePage.xaml line 38
                    if (!isobj7CommandDisabled)
                    {
                        XamlBindingSetters.Set_Microsoft_UI_Xaml_Controls_Primitives_ButtonBase_Command(this.obj7, obj, null);
                    }
                }
            }
            private void Update_ViewModel_SelectedImageName(global::System.String obj, int phase)
            {
                if ((phase & ((1 << 0) | NOT_PHASED | DATA_CHANGED)) != 0)
                {
                    // Views\ConfigurationsView\ModifyProfilePage.xaml line 40
                    if (!isobj8TextDisabled)
                    {
                        XamlBindingSetters.Set_Microsoft_UI_Xaml_Controls_TextBlock_Text(this.obj8, obj, null);
                    }
                }
            }
            private void UpdateTwoWay_3_Text()
            {
                if (this.initialized)
                {
                    if (this.dataRoot != null)
                    {
                        if (this.dataRoot.ViewModel != null)
                        {
                            this.dataRoot.ViewModel.Description = this.obj3.Text;
                        }
                    }
                }
            }

            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.UI.Xaml.Markup.Compiler"," 3.0.0.2312")]
            [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
            private class ModifyProfilePage_obj1_BindingsTracking
            {
                private global::System.WeakReference<ModifyProfilePage_obj1_Bindings> weakRefToBindingObj; 

                public ModifyProfilePage_obj1_BindingsTracking(ModifyProfilePage_obj1_Bindings obj)
                {
                    weakRefToBindingObj = new global::System.WeakReference<ModifyProfilePage_obj1_Bindings>(obj);
                }

                public ModifyProfilePage_obj1_Bindings TryGetBindingObject()
                {
                    ModifyProfilePage_obj1_Bindings bindingObject = null;
                    if (weakRefToBindingObj != null)
                    {
                        weakRefToBindingObj.TryGetTarget(out bindingObject);
                        if (bindingObject == null)
                        {
                            weakRefToBindingObj = null;
                            ReleaseAllListeners();
                        }
                    }
                    return bindingObject;
                }

                public void ReleaseAllListeners()
                {
                    UpdateChildListeners_ViewModel(null);
                }

                public void PropertyChanged_ViewModel(object sender, global::System.ComponentModel.PropertyChangedEventArgs e)
                {
                    ModifyProfilePage_obj1_Bindings bindings = TryGetBindingObject();
                    if (bindings != null)
                    {
                        string propName = e.PropertyName;
                        global::SteamProfile.ViewModels.ConfigurationsViewModels.ModifyProfileViewModel obj = sender as global::SteamProfile.ViewModels.ConfigurationsViewModels.ModifyProfileViewModel;
                        if (global::System.String.IsNullOrEmpty(propName))
                        {
                            if (obj != null)
                            {
                                bindings.Update_ViewModel_SuccessMessage(obj.SuccessMessage, DATA_CHANGED);
                                bindings.Update_ViewModel_SuccessMessageVisibility(obj.SuccessMessageVisibility, DATA_CHANGED);
                                bindings.Update_ViewModel_Description(obj.Description, DATA_CHANGED);
                                bindings.Update_ViewModel_DescriptionErrorMessage(obj.DescriptionErrorMessage, DATA_CHANGED);
                                bindings.Update_ViewModel_DescriptionErrorVisibility(obj.DescriptionErrorVisibility, DATA_CHANGED);
                                bindings.Update_ViewModel_CanSave(obj.CanSave, DATA_CHANGED);
                                bindings.Update_ViewModel_SelectedImageName(obj.SelectedImageName, DATA_CHANGED);
                            }
                        }
                        else
                        {
                            switch (propName)
                            {
                                case "SuccessMessage":
                                {
                                    if (obj != null)
                                    {
                                        bindings.Update_ViewModel_SuccessMessage(obj.SuccessMessage, DATA_CHANGED);
                                    }
                                    break;
                                }
                                case "SuccessMessageVisibility":
                                {
                                    if (obj != null)
                                    {
                                        bindings.Update_ViewModel_SuccessMessageVisibility(obj.SuccessMessageVisibility, DATA_CHANGED);
                                    }
                                    break;
                                }
                                case "Description":
                                {
                                    if (obj != null)
                                    {
                                        bindings.Update_ViewModel_Description(obj.Description, DATA_CHANGED);
                                    }
                                    break;
                                }
                                case "DescriptionErrorMessage":
                                {
                                    if (obj != null)
                                    {
                                        bindings.Update_ViewModel_DescriptionErrorMessage(obj.DescriptionErrorMessage, DATA_CHANGED);
                                    }
                                    break;
                                }
                                case "DescriptionErrorVisibility":
                                {
                                    if (obj != null)
                                    {
                                        bindings.Update_ViewModel_DescriptionErrorVisibility(obj.DescriptionErrorVisibility, DATA_CHANGED);
                                    }
                                    break;
                                }
                                case "CanSave":
                                {
                                    if (obj != null)
                                    {
                                        bindings.Update_ViewModel_CanSave(obj.CanSave, DATA_CHANGED);
                                    }
                                    break;
                                }
                                case "SelectedImageName":
                                {
                                    if (obj != null)
                                    {
                                        bindings.Update_ViewModel_SelectedImageName(obj.SelectedImageName, DATA_CHANGED);
                                    }
                                    break;
                                }
                                default:
                                    break;
                            }
                        }
                    }
                }
                private global::SteamProfile.ViewModels.ConfigurationsViewModels.ModifyProfileViewModel cache_ViewModel = null;
                public void UpdateChildListeners_ViewModel(global::SteamProfile.ViewModels.ConfigurationsViewModels.ModifyProfileViewModel obj)
                {
                    if (obj != cache_ViewModel)
                    {
                        if (cache_ViewModel != null)
                        {
                            ((global::System.ComponentModel.INotifyPropertyChanged)cache_ViewModel).PropertyChanged -= PropertyChanged_ViewModel;
                            cache_ViewModel = null;
                        }
                        if (obj != null)
                        {
                            cache_ViewModel = obj;
                            ((global::System.ComponentModel.INotifyPropertyChanged)obj).PropertyChanged += PropertyChanged_ViewModel;
                        }
                    }
                }
                public void RegisterTwoWayListener_3(global::Microsoft.UI.Xaml.Controls.TextBox sourceObject)
                {
                    sourceObject.RegisterPropertyChangedCallback(global::Microsoft.UI.Xaml.Controls.TextBox.TextProperty, (sender, prop) =>
                    {
                        var bindingObj = this.TryGetBindingObject();
                        if (bindingObj != null)
                        {
                            bindingObj.UpdateTwoWay_3_Text();
                        }
                    });
                }
            }
        }

        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.UI.Xaml.Markup.Compiler"," 3.0.0.2312")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 6: // Views\ConfigurationsView\ModifyProfilePage.xaml line 67
                {
                    global::Microsoft.UI.Xaml.Controls.Button element6 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)element6).Click += this.GoBack;
                }
                break;
            case 7: // Views\ConfigurationsView\ModifyProfilePage.xaml line 38
                {
                    this.PickAPhotoButton = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                }
                break;
            case 8: // Views\ConfigurationsView\ModifyProfilePage.xaml line 40
                {
                    this.PickAPhotoOutputTextBlock = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.TextBlock>(target);
                }
                break;
            default:
                break;
            }
            this._contentLoaded = true;
        }

        /// <summary>
        /// GetBindingConnector(int connectionId, object target)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.UI.Xaml.Markup.Compiler"," 3.0.0.2312")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Microsoft.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Microsoft.UI.Xaml.Markup.IComponentConnector returnValue = null;
            switch(connectionId)
            {
            case 1: // Views\ConfigurationsView\ModifyProfilePage.xaml line 2
                {                    
                    global::Microsoft.UI.Xaml.Controls.Page element1 = (global::Microsoft.UI.Xaml.Controls.Page)target;
                    ModifyProfilePage_obj1_Bindings bindings = new ModifyProfilePage_obj1_Bindings();
                    returnValue = bindings;
                    bindings.SetDataRoot(this);
                    this.Bindings = bindings;
                    element1.Loading += bindings.Loading;
                    global::Microsoft.UI.Xaml.Markup.XamlBindingHelper.SetDataTemplateComponent(element1, bindings);
                }
                break;
            }
            return returnValue;
        }
    }
}

