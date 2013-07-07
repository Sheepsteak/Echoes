using Caliburn.Micro;
using Microsoft.Phone.Controls;
using Sheepsteak.Echo.Features.Articles;
using Sheepsteak.Echo.Features.Main;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Controls;

namespace Sheepsteak.Echo
{
    public class AppBootstrapper : PhoneBootstrapperBase
    {
        private PhoneContainer container;

        public AppBootstrapper()
        {
            Start();
        }

        protected override void Configure()
        {
            this.container = new PhoneContainer();
            if (!Execute.InDesignMode)
            {
                this.container.RegisterPhoneServices(this.RootFrame);
            }

            this.container.PerRequest<ArticlePageViewModel>();
            this.container.PerRequest<MainPageViewModel>();
            this.container.Singleton<EchoJsClient>();

            AddCustomConventions();
        }

        protected override object GetInstance(Type service, string key)
        {
            var instance = this.container.GetInstance(service, key);
            if (instance != null)
                return instance;

            throw new InvalidOperationException("Could not locate any instances.");
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return this.container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            this.container.BuildUp(instance);
        }

        private static void AddCustomConventions()
        {
            ConventionManager.AddElementConvention<LongListSelector>(LongListSelector.ItemsSourceProperty, "SelectedItem", "SelectionChanged").ApplyBinding =
                (viewModelType, path, property, element, convention) =>
                {
                    if (!ConventionManager.SetBindingWithoutBindingOrValueOverwrite(
                        viewModelType, path, property, element, convention, LongListSelector.ItemsSourceProperty))
                    {
                        return false;
                    }

                    ApplyLongListSelectorItemTemplate((LongListSelector)element, property);

                    return true;
                    //if (ConventionManager
                    //    .GetElementConvention(typeof(ItemsControl))
                    //    .ApplyBinding(viewModelType, path, property, element, convention))
                    //{
                    //    ConventionManager
                    //        .ConfigureSelectedItem(element, Pivot.SelectedItemProperty, viewModelType, path);

                    //    return true;
                    //}
                };

            ConventionManager.AddElementConvention<Pivot>(Pivot.ItemsSourceProperty, "SelectedItem", "SelectionChanged").ApplyBinding =
                (viewModelType, path, property, element, convention) =>
                {
                    if (ConventionManager
                        .GetElementConvention(typeof(ItemsControl))
                        .ApplyBinding(viewModelType, path, property, element, convention))
                    {
                        ConventionManager
                            .ConfigureSelectedItem(element, Pivot.SelectedItemProperty, viewModelType, path);
                        ConventionManager
                            .ApplyHeaderTemplate(element, Pivot.HeaderTemplateProperty, null, viewModelType);
                        return true;
                    }

                    return false;
                };

            ConventionManager.AddElementConvention<Panorama>(Panorama.ItemsSourceProperty, "SelectedItem", "SelectionChanged").ApplyBinding =
                (viewModelType, path, property, element, convention) =>
                {
                    if (ConventionManager
                        .GetElementConvention(typeof(ItemsControl))
                        .ApplyBinding(viewModelType, path, property, element, convention))
                    {
                        ConventionManager
                            .ConfigureSelectedItem(element, Panorama.SelectedItemProperty, viewModelType, path);
                        ConventionManager
                            .ApplyHeaderTemplate(element, Panorama.HeaderTemplateProperty, null, viewModelType);
                        return true;
                    }

                    return false;
                };
        }

        /// <summary>
        /// Attempts to apply the default item template to the items control.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="property">The collection property.</param>
        private static void ApplyLongListSelectorItemTemplate(LongListSelector longlistSelector, PropertyInfo property)
        {
            if (longlistSelector.ItemTemplate != null)
            {
                return;
            }

            longlistSelector.ItemTemplate = ConventionManager.DefaultItemTemplate;
        }
    }
}