using WebExpress.WebApp.WebScope;
using WebExpress.WebApp.WebSettingPage;
using WebExpress.WebCore;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebPage;
using WebExpress.WebCore.WebSettingPage;
using WebExpress.WebCore.WebUri;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebIcon;

namespace ViLa.WWW.Settings
{
    /// <summary>
    /// Provides portable data backups and additive imports in application settings.
    /// </summary>
    [Title("vila:vila.backup.title")]
    [WebIcon<IconDatabase>]
    [Application<Application>]
    [Scope<IScopeGeneral>]
    public sealed class Data : ISettingPage<VisualTreeWebAppSetting>, IScopeGeneral
    {
        /// <summary>
        /// Builds responsive export and import cards using standard WebExpress controls.
        /// </summary>
        /// <param name="renderContext">The request and application routing context.</param>
        /// <param name="visualTree">The settings page receiving the controls.</param>
        public void Process(IRenderContext renderContext, VisualTreeWebAppSetting visualTree)
        {
            var panel = new ControlPanel("backup_page")
            {
                Padding = _ => new PropertySpacingPadding(PropertySpacing.Space.Three),
                Styles = new[] { "max-width: 1200px" }
            };
            panel.Add(new ControlText
            {
                Text = _ => "vila:vila.backup.intro",
                Margin = _ => new PropertySpacingMargin(PropertySpacing.Space.None, PropertySpacing.Space.None,
                    PropertySpacing.Space.Four, PropertySpacing.Space.None)
            });
            var grid = new ControlGrid("backup_actions");
            var export = new ControlCard("backup_export")
            {
                Header = _ => "vila:vila.backup.export.title",
                HeaderIcon = _ => new IconDownload()
            };
            export.Add(new ControlText { Text = _ => "vila:vila.backup.export.help" });
            var downloads = new ControlPanel
            {
                Display = _ => TypeDisplay.Grid,
                Styles = new[] { "gap: 0.75rem" },
                Margin = _ => new PropertySpacingMargin(PropertySpacing.Space.Three, PropertySpacing.Space.None)
            };
            downloads.Add(new ControlButtonLink("backup_download")
            {
                Text = _ => "vila:vila.backup.export",
                Icon = _ => new IconDownload(),
                BackgroundColor = _ => new PropertyColorButton(TypeColorButton.Primary),
                Uri = _ => WebEx.ComponentHub.SitemapManager.GetUri<Api._1_.Backup>(renderContext.Request.ApplicationContext)
            });
            downloads.Add(new ControlButtonLink("backup_download_archive")
            {
                Text = _ => "vila:vila.backup.export.archive",
                Icon = _ => new IconBoxArchive(),
                BackgroundColor = _ => new PropertyColorButton(TypeColorButton.Primary),
                Outline = _ => true,
                Uri = _ => new UriEndpoint(WebEx.ComponentHub.SitemapManager
                    .GetUri<Api._1_.Backup>(renderContext.Request.ApplicationContext) + "?includeArchive=true")
            });
            export.Add(downloads);
            export.Add(new ControlAlert
            {
                Text = _ => "vila:vila.backup.running",
                BackgroundColor = _ => new PropertyColorBackgroundAlert(TypeColorBackgroundAlert.Light),
                Dismissibility = _ => TypeDismissibilityAlert.None
            });
            var import = new ControlCard("backup_import")
            {
                Header = _ => "vila:vila.backup.import.title",
                HeaderIcon = _ => new IconUpload()
            };
            import.Add(new ControlText { Text = _ => "vila:vila.backup.import.help" });
            import.Add(new ControlAlert
            {
                Head = _ => "vila:vila.backup.preserve.title",
                Text = _ => "vila:vila.backup.preserve.help",
                BackgroundColor = _ => new PropertyColorBackgroundAlert(TypeColorBackgroundAlert.Info),
                Dismissibility = _ => TypeDismissibilityAlert.None
            });
            import.Add(new ControlUpload("backup_upload")
            {
                Classes = new[] { "vila-backup-upload" },
                Placeholder = _ => "vila:vila.backup.drop",
                Accept = _ => ".zip,application/zip",
                Multiple = _ => false,
                AutoUpload = _ => false,
                Uri = _ => WebEx.ComponentHub.SitemapManager.GetUri<Api._1_.Backup>(renderContext.Request.ApplicationContext)
            });
            import.Add(new ControlText
            {
                Text = _ => "vila:vila.backup.file.help",
                Format = _ => TypeFormatText.Small
            });
            import.Add(new ControlAlert("backup_result_success")
            {
                Head = _ => "vila:vila.backup.result.success",
                Text = _ => "vila:vila.backup.import.done",
                Display = _ => TypeDisplay.None,
                BackgroundColor = _ => new PropertyColorBackgroundAlert(TypeColorBackgroundAlert.Success),
                Dismissibility = _ => TypeDismissibilityAlert.None
            });
            import.Add(new ControlAlert("backup_result_error")
            {
                Head = _ => "vila:vila.backup.result.error",
                Text = _ => "vila:vila.backup.invalid",
                Display = _ => TypeDisplay.None,
                BackgroundColor = _ => new PropertyColorBackgroundAlert(TypeColorBackgroundAlert.Danger),
                Dismissibility = _ => TypeDismissibilityAlert.None
            });
            import.Add(new ControlAlert("backup_result_unavailable")
            {
                Head = _ => "vila:vila.backup.result.error",
                Text = _ => "vila:vila.backup.unavailable",
                Display = _ => TypeDisplay.None,
                BackgroundColor = _ => new PropertyColorBackgroundAlert(TypeColorBackgroundAlert.Danger),
                Dismissibility = _ => TypeDismissibilityAlert.None
            });
            foreach (var card in new[] { export, import })
            {
                var column = new ControlPanel
                {
                    GridColumn = _ => new PropertyGrid(TypeDevice.Large, 6),
                    Margin = _ => new PropertySpacingMargin(PropertySpacing.Space.None, PropertySpacing.Space.None,
                        PropertySpacing.Space.Three, PropertySpacing.Space.None)
                };
                column.Add(card);
                grid.Add(column);
            }
            panel.Add(grid);
            visualTree.Content.MainPanel.AddPrimary(panel);
        }
    }
}
