using System;
using System.Drawing;
using System.Windows.Forms;
using TextEditorMK.Models;

namespace TextEditorMK.Helpers
{

    public static class ThemeHelper
    {
        public static void ApplyThemeToForm(Form form, EditorTheme theme)
        {
            if (form == null || theme == null) return;

            try
            {
                form.BackColor = theme.BackgroundColor;
                form.ForeColor = theme.ForegroundColor;

                ApplyThemeToControls(form.Controls, theme);

                System.Diagnostics.Debug.WriteLine($"? Applied {theme.Name} theme to form {form.Name}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"? Error applying theme to form: {ex.Message}");
            }
        }


        public static void ApplyThemeToControls(Control.ControlCollection controls, EditorTheme theme)
        {
            foreach (Control control in controls)
            {
                ApplyThemeToControl(control, theme);
            }
        }

        public static void ApplyThemeToControl(Control control, EditorTheme theme)
        {
            if (control == null || theme == null) return;

            try
            {
                switch (control)
                {
                    case Button button:
                        ApplyThemeToButton(button, theme);
                        break;

                    case TextBox textBox:
                        ApplyThemeToTextBox(textBox, theme);
                        break;

                    case RichTextBox richTextBox:
                        ApplyThemeToRichTextBox(richTextBox, theme);
                        break;

                    case ComboBox comboBox:
                        ApplyThemeToComboBox(comboBox, theme);
                        break;

                    case ListBox listBox:
                        ApplyThemeToListBox(listBox, theme);
                        break;

                    case ListView listView:
                        ApplyThemeToListView(listView, theme);
                        break;

                    case TreeView treeView:
                        ApplyThemeToTreeView(treeView, theme);
                        break;

                    case NumericUpDown numericUpDown:
                        ApplyThemeToNumericUpDown(numericUpDown, theme);
                        break;

                    case CheckBox checkBox:
                        ApplyThemeToCheckBox(checkBox, theme);
                        break;

                    case RadioButton radioButton:
                        ApplyThemeToRadioButton(radioButton, theme);
                        break;

                    case GroupBox groupBox:
                        ApplyThemeToGroupBox(groupBox, theme);
                        break;

                    case TabControl tabControl:
                        ApplyThemeToTabControl(tabControl, theme);
                        break;

                    case Label label:
                        ApplyThemeToLabel(label, theme);
                        break;

                    case Panel panel:
                        ApplyThemeToPanel(panel, theme);
                        break;

                    case DataGridView dataGridView:
                        ApplyThemeToDataGridView(dataGridView, theme);
                        break;

                    default:
                        control.BackColor = theme.BackgroundColor;
                        control.ForeColor = theme.ForegroundColor;
                        break;
                }

                if (control.HasChildren)
                {
                    ApplyThemeToControls(control.Controls, theme);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"?? Error applying theme to control {control.Name}: {ex.Message}");
            }
        }

        private static void ApplyThemeToButton(Button button, EditorTheme theme)
        {
            button.BackColor = theme.ButtonBackColor;
            button.ForeColor = theme.ButtonForeColor;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderColor = theme.ButtonBorderColor;
            button.FlatAppearance.BorderSize = 1;

            button.MouseEnter -= Button_MouseEnter;
            button.MouseLeave -= Button_MouseLeave;

            button.MouseEnter += Button_MouseEnter;
            button.MouseLeave += Button_MouseLeave;

            button.Tag = theme;
        }

        private static void Button_MouseEnter(object sender, EventArgs e)
        {
            if (sender is Button button && button.Tag is EditorTheme theme)
            {
                button.BackColor = theme.HoverColor;
            }
        }

        private static void Button_MouseLeave(object sender, EventArgs e)
        {
            if (sender is Button button && button.Tag is EditorTheme theme)
            {
                button.BackColor = theme.ButtonBackColor;
            }
        }

        private static void ApplyThemeToTextBox(TextBox textBox, EditorTheme theme)
        {
            textBox.BackColor = theme.TextBoxBackColor;
            textBox.ForeColor = theme.TextBoxForeColor;
            
            if (theme.Name == "Dark")
            {
                textBox.BorderStyle = BorderStyle.FixedSingle;
            }
        }

        private static void ApplyThemeToRichTextBox(RichTextBox richTextBox, EditorTheme theme)
        {
            richTextBox.BackColor = theme.TextBoxBackColor;
            richTextBox.ForeColor = theme.TextBoxForeColor;
        }

        private static void ApplyThemeToComboBox(ComboBox comboBox, EditorTheme theme)
        {
            if (theme.Name == "Dark")
            {
                comboBox.BackColor = theme.TextBoxBackColor;
                comboBox.ForeColor = theme.TextBoxForeColor;
                comboBox.FlatStyle = FlatStyle.Flat;
            }
            else
            {
                comboBox.BackColor = Color.White;
                comboBox.ForeColor = Color.Black;
                comboBox.FlatStyle = FlatStyle.Standard;
            }
        }

        private static void ApplyThemeToListBox(ListBox listBox, EditorTheme theme)
        {
            listBox.BackColor = theme.TextBoxBackColor;
            listBox.ForeColor = theme.TextBoxForeColor;
            
            if (theme.Name == "Dark")
            {
                listBox.BorderStyle = BorderStyle.FixedSingle;
            }
        }

        private static void ApplyThemeToListView(ListView listView, EditorTheme theme)
        {
            listView.BackColor = theme.TextBoxBackColor;
            listView.ForeColor = theme.TextBoxForeColor;
            
            if (theme.Name == "Dark")
            {
                listView.BorderStyle = BorderStyle.FixedSingle;
            }
        }

        private static void ApplyThemeToTreeView(TreeView treeView, EditorTheme theme)
        {
            treeView.BackColor = theme.TextBoxBackColor;
            treeView.ForeColor = theme.TextBoxForeColor;
            
            if (theme.Name == "Dark")
            {
                treeView.BorderStyle = BorderStyle.FixedSingle;
                treeView.LineColor = theme.BorderColor;
            }
        }

        private static void ApplyThemeToNumericUpDown(NumericUpDown numericUpDown, EditorTheme theme)
        {
            numericUpDown.BackColor = theme.TextBoxBackColor;
            numericUpDown.ForeColor = theme.TextBoxForeColor;
            
            if (theme.Name == "Dark")
            {
                numericUpDown.BorderStyle = BorderStyle.FixedSingle;
            }
        }

        private static void ApplyThemeToCheckBox(CheckBox checkBox, EditorTheme theme)
        {
            checkBox.BackColor = theme.BackgroundColor;
            checkBox.ForeColor = theme.ForegroundColor;
        }

        private static void ApplyThemeToRadioButton(RadioButton radioButton, EditorTheme theme)
        {
            radioButton.BackColor = theme.BackgroundColor;
            radioButton.ForeColor = theme.ForegroundColor;
        }

        private static void ApplyThemeToGroupBox(GroupBox groupBox, EditorTheme theme)
        {
            groupBox.BackColor = theme.BackgroundColor;
            groupBox.ForeColor = theme.ForegroundColor;
        }

        private static void ApplyThemeToTabControl(TabControl tabControl, EditorTheme theme)
        {
            tabControl.BackColor = theme.BackgroundColor;
            tabControl.ForeColor = theme.ForegroundColor;
            
            // Застосувати до всіх вкладок
            foreach (TabPage tabPage in tabControl.TabPages)
            {
                tabPage.BackColor = theme.BackgroundColor;
                tabPage.ForeColor = theme.ForegroundColor;
            }
        }

        private static void ApplyThemeToLabel(Label label, EditorTheme theme)
        {
            label.BackColor = Color.Transparent; // Прозорий фон для Label
            label.ForeColor = theme.ForegroundColor;
        }

        private static void ApplyThemeToPanel(Panel panel, EditorTheme theme)
        {
            panel.BackColor = theme.BackgroundColor;
            panel.ForeColor = theme.ForegroundColor;
        }

        private static void ApplyThemeToDataGridView(DataGridView dataGridView, EditorTheme theme)
        {
            dataGridView.BackgroundColor = theme.BackgroundColor;
            dataGridView.DefaultCellStyle.BackColor = theme.TextBoxBackColor;
            dataGridView.DefaultCellStyle.ForeColor = theme.TextBoxForeColor;
            dataGridView.DefaultCellStyle.SelectionBackColor = theme.SelectionBackColor;
            dataGridView.DefaultCellStyle.SelectionForeColor = theme.SelectionForeColor;
            
            dataGridView.ColumnHeadersDefaultCellStyle.BackColor = theme.MenuBackColor;
            dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = theme.MenuForeColor;
            
            dataGridView.RowHeadersDefaultCellStyle.BackColor = theme.MenuBackColor;
            dataGridView.RowHeadersDefaultCellStyle.ForeColor = theme.MenuForeColor;
            
            if (theme.Name == "Dark")
            {
                dataGridView.BorderStyle = BorderStyle.FixedSingle;
                dataGridView.GridColor = theme.BorderColor;
            }
        }

        public static ToolStripRenderer CreateDarkRenderer(EditorTheme theme)
        {
            return new DarkToolStripRenderer(theme);
        }
    }

    public class DarkToolStripRenderer : ToolStripProfessionalRenderer
    {
        private readonly EditorTheme _theme;

        public DarkToolStripRenderer(EditorTheme theme)
        {
            _theme = theme ?? throw new ArgumentNullException(nameof(theme));
        }

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            using (var brush = new SolidBrush(_theme.MenuBackColor))
            {
                e.Graphics.FillRectangle(brush, e.AffectedBounds);
            }
        }

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            Color backgroundColor = e.Item.Selected || e.Item.Pressed ? _theme.HoverColor : _theme.MenuBackColor;
            
            using (var brush = new SolidBrush(backgroundColor))
            {
                e.Graphics.FillRectangle(brush, e.Item.ContentRectangle);
            }
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            e.TextColor = _theme.MenuForeColor;
            base.OnRenderItemText(e);
        }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
        }

        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            using (var pen = new Pen(_theme.BorderColor))
            {
                var bounds = e.Item.Bounds;
                int y = bounds.Top + bounds.Height / 2;
                e.Graphics.DrawLine(pen, bounds.Left + 20, y, bounds.Right - 5, y);
            }
        }

        protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
        {
            if (e.Item is ToolStripButton button)
            {
                Color backgroundColor = button.Pressed ? _theme.SelectionBackColor :
                                      button.Selected ? _theme.HoverColor : _theme.MenuBackColor;
                
                using (var brush = new SolidBrush(backgroundColor))
                {
                    e.Graphics.FillRectangle(brush, e.Item.Bounds);
                }
            }
        }
    }
}