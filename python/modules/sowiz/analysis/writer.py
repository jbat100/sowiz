import essentia.standard


class AudioFeatureExtractor(object):
	
	def __init__(self, audio, file_handle):
		self.__audio = audio
		self.__file_handle = file_handle
		self.__extractor_config = {}
		
	def get_extractor_config(self):
		return self.__extractor_config
		
	def get_audio(self):
		return self.__audio
	
	