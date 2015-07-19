
from math import ceil

import wx

from sowiz.network.osc import Client
from sowiz.wxgui.core import SlidersModule

__author__ = 'jonathanthorpe'


class OSCControllerModule(SlidersModule):

	def __init__(self, panel, path, nargs):
		super(OSCControllerModule, self).__init__(panel)
		self.__path = path
		self.__nargs = nargs
		self.__client = None

	@property
	def path(self):
		return self.__path

	@property
	def nargs(self):
		return self.__nargs

	@property
	def rows(self):
		return int(ceil(self.nargs / self.columns))

	@property
	def client(self):
		return self.__client

	def start(self, host, port):
		self.__client = Client(host, port)

	def make(self):

		vbox = wx.BoxSizer(wx.VERTICAL)
		vbox.Add( super(OSCControllerModule, self).make(), flag=wx.ALIGN_CENTER|wx.EXPAND, proportion=1.0)

		b = wx.Button(self.panel, label="Send")
		self.vbox.Add(b)
		b.Bind(wx.EVT_BUTTON, self.on_send)







