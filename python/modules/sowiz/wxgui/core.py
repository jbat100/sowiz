from sowiz.util import variable_type_check

import wx

__author__ = 'jonathanthorpe'


class Module(object):

	""" Module subclasses should handle both the GUI creation and call-backs
	hence we need a reference to the application to be able to do stuff"""

	def __init__(self, panel):
		variable_type_check(panel, wx.Panel)
		self.__panel = panel

	@property
	def panel(self):
		return self.__panel


class SlidersModule(Module):

	def __init__(self, *args, **kwargs):
		super(SlidersModule, self).__init__(*args, **kwargs)
		self.__grid_sizer = wx.GridSizer(rows=self.rows, cols=self.columns, vgap=10, hgap=10)

	@property
	def grid_sizer(self):
		return self.__grid_sizer

	@property
	def rows(self):
		return 1

	@property
	def columns(self):
		return 4

	def add_slider(self, callback, value_range=(0.0, 1.0) , default_value=0.0):
		s = wx.Slider(self.panel, -1, default_value,  value_range[0], value_range[1])
		self.__grid_sizer.Add(s, flag=wx.EXPAND, proportion=1.0)
		s.Bind(wx.EVT_SLIDER, callback)
		return s

	def make(self):
		self.make_sliders()
		return self.grid_sizer
