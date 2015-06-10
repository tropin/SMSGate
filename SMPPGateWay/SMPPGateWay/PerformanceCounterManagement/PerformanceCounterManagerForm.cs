using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Csharper.SMS.PerformanceCounterManagment
{
	public partial class PerformanceCounterManagerForm : Form
	{
		private bool IsGroupExists
		{
			get { return PerformanceCounterCategory.Exists(tbGroupName.Text); }
		}

		public PerformanceCounterManagerForm()
		{
			InitializeComponent();
		}

		private void PerformanceCounterManagerForm_Load(object sender, EventArgs e)
		{
			bCreate.Enabled = !IsGroupExists;
			bDelete.Enabled = IsGroupExists;

			cbType.DataSource = Enum.GetValues(typeof(PerformanceCounterType));
			cbType.SelectedIndex = 2;

			if (IsGroupExists)
			{
				FillExistsList();

				gbExists.Enabled = true;
				gbAddNew.Enabled = true;
			}
		}

		private void BCreateClick(object sender, EventArgs e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;

				PerformanceCounterCategory.Create(tbGroupName.Text, tbGroupName.Text,
					PerformanceCounterCategoryType.SingleInstance, new CounterCreationDataCollection());

				bDelete.Enabled = true;
				bCreate.Enabled = false;
				gbExists.Enabled = true;
				gbAddNew.Enabled = true;
			}
			catch(Exception ex)
			{
				MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				Cursor = DefaultCursor;
			}
		}

		private void BDeleteClick(object sender, EventArgs e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;

				lbExists.DataSource = null;

				PerformanceCounterCategory.Delete(tbGroupName.Text);

				bDelete.Enabled = false;
				bCreate.Enabled = true;
				gbExists.Enabled = false;
				gbAddNew.Enabled = false;
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				Cursor = DefaultCursor;
			}
		}

		private void BAddCounterClick(object sender, EventArgs e)
		{
			try
			{
				if (string.IsNullOrEmpty(tbName.Text) || string.IsNullOrEmpty(tbDescription.Text) || cbType.SelectedIndex == -1)
				{
					MessageBox.Show(this, "Specify name and description!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

					return;
				}

				Cursor = Cursors.WaitCursor;

				PerformanceCounterCategory category = new PerformanceCounterCategory(tbGroupName.Text);

				IEnumerable<CounterCreationData> existsCounters = category.GetCounters(string.Empty)
					.Select(x => new CounterCreationData(x.CounterName, x.CounterHelp, x.CounterType));

				CounterCreationDataCollection counters = new CounterCreationDataCollection(existsCounters.ToArray());

				CounterCreationData newCounterCreationData = new CounterCreationData
					{
						CounterName = tbName.Text,
						CounterHelp = tbDescription.Text,
						CounterType = (PerformanceCounterType) cbType.SelectedItem
					};

				counters.Add(newCounterCreationData);

				PerformanceCounterCategory.Delete(tbGroupName.Text);

				PerformanceCounterCategory.Create(tbGroupName.Text, tbGroupName.Text,
					PerformanceCounterCategoryType.SingleInstance, counters);

				FillExistsList();

				tbName.Text = string.Empty;
				tbDescription.Text = string.Empty;
				cbType.SelectedIndex = 2;
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				Cursor = DefaultCursor;
			}
		}

		private void BDeleteSelectedClick(object sender, EventArgs e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;

				PerformanceCounterCategory category = new PerformanceCounterCategory(tbGroupName.Text);

				IEnumerable<CounterCreationData> existsCounters = category.GetCounters(string.Empty)
					.Where(x => x.CounterName != ((CounterContainer)lbExists.SelectedItem).Name)
					.Select(x => new CounterCreationData(x.CounterName, x.CounterHelp, x.CounterType));

				CounterCreationDataCollection counters = new CounterCreationDataCollection(existsCounters.ToArray());

				PerformanceCounterCategory.Delete(tbGroupName.Text);

				PerformanceCounterCategory.Create(tbGroupName.Text, tbGroupName.Text,
					PerformanceCounterCategoryType.SingleInstance, counters);

				FillExistsList();
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				Cursor = DefaultCursor;
			}
		}

		private void LbExistsSelectedIndexChanged(object sender, EventArgs e)
		{
			bDeleteSelected.Enabled = lbExists.SelectedIndex != -1;
		}

		private void FillExistsList()
		{
			try
			{
				Cursor = Cursors.WaitCursor;

				PerformanceCounterCategory category = new PerformanceCounterCategory(tbGroupName.Text);

				IEnumerable<CounterContainer> counters = category.GetCounters(string.Empty).Select(x => new CounterContainer(x));

				lbExists.DataSource = counters.ToList();
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				Cursor = DefaultCursor;
			}
		}
	}
}
