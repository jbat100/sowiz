
import logging

import mido

from sowiz.network.osc import Message
from sowiz.util import variable_type_check
from sowiz.description.core import Event, EventFileReader

class MidiMessage(Event):

	def __init__(self, identifier, time_stamp):
		super(MidiMessage, self).__init__(identifier, time_stamp)
		self.__mido_message = None

	def __str__(self):
		s = super(MidiMessage, self).__str__()
		return s

	def __getattr__(self, item):
		if item in [ 'channel', 'note', 'velocity', 'program', 'control', 'value' ]:
			return getattr( self.__mido_message, item )
		else:
			raise AttributeError('unknown attribute : %s', item)

	@property
	def type(self):
		return self.__mido_message.type

	@property
	def attributes(self):
		if self.type == 'program_change':
			return ['channel', 'program']
		elif self.type == 'control_change':
			return ['channel', 'control', 'value']
		elif self.type in ['note_on', 'note_off']:
			return ['channel', 'note', 'velocity']

	@property
	def values(self):
		return [getattr(self, a) for a in self.attributes]

	@classmethod
	def new_from_mido_message(cls, identifier, time_stamp, mido_message):
		variable_type_check(mido_message, mido.Message)
		midi_message = cls(identifier, time_stamp)
		midi_message.__mido_message = mido_message
		return midi_message


class MidiOSCTranslator(object):

	EVENT_TYPE = MidiMessage

	PATH_PREFIX = '/sowiz/midi/'

	def __init__(self):
		pass

	def translate(self, midi_event):
		variable_type_check(midi_event, MidiMessage)
		path = self.PATH_PREFIX + midi_event.type
		message = Message(path, [midi_event.type] + midi_event.attributes )
		return message


class MidiFileReader(EventFileReader):

	EXPECTED_EXTENSIONS = ['.midi', '.mid']

	def __init__(self, identifier, file_path):
		super(MidiFileReader, self).__init__(identifier, file_path)

	@property
	def events(self):
		mido_messages = mido.MidiFile(self.file_path)
		time_stamp = 0.0
		for mido_message in mido_messages:
			time_stamp += mido_message.time
			if isinstance(mido_message, mido.MetaMessage):
				logging.debug('read meta message %s' % mido_message)
				continue
			# time from mido is relative to last, so we add it on to the current time stamp
			message = MidiMessage.new_from_mido_message(self.identifier, time_stamp, mido_message)
			yield message



