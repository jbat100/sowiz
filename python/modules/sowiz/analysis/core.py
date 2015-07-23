import json
import essentia.standard


 # salut c'est jonathan


class FeatureExtractor(object):
	
	def __init__(self, audio, writer):
		self.__audio = audio
		self.__writer = writer
		self.__frame_processors = []
		
	def get_audio(self):
		return self.__audio
	
	def add_frame_processor(self, frame_processor):
		self.__frame_processors.append(frame_processor)
	
	def extract(self, frame_size = 1024, hop_size = 512):
		count = 0
		for frame in essentia.standard.FrameGenerator(self.get_audio(), frame_size = frame_size, hop_size = hop_size):
			count += 1
			b = bands(spec(w(frame)))
			print type(b), ' ', b
			if count%100 == 0: print 'processed %d frames'%count
			if count > 2000: break


class FrameProcessor(object):
	
	def __init__(self, identifier):
		self.__identifier = identifier
		
	def descriptor(self):
		raise NotImplementedError()
		
	def process(self, frame):
		raise NotImplementedError()


class SpectralBandsProcessor(FrameProcessor):
	
	def __init__(self, identifier, bands=12):
		super(SpectralBandsProcessor, self).__init__(identifier)
		self.windowing = essentia.standard.Windowing()
		self.spectrum = essentia.standard.Spectrum()
		self.bands = essentia.standard.ERBBands(numberBands = bands)
		
	def descriptor(self):
		return self.__class__.__name__ + ' ' + str({'bands' : self.bands.paramValue('numberBands')})
		
	def process(self, frame):
		return self.bands(self.spectrum(self.windowing(frame)))
	
