
import os

from sowiz.util import Enum

AnnotationTypes = Enum(['BEAT_SPECTRAL_DIFF',
						'RMS_AMPLITUDE',
						'CHRONOGRAM',
						'SPECTRAL_CENTROID',
						'CONSTANT_Q_SPECT'])


AnnotationFileSuffixes = {
	AnnotationTypes.BEAT_SPECTRAL_DIFF 	: '_vamp_qm-vamp-plugins_qm-barbeattracker_beatsd',
	AnnotationTypes.RMS_AMPLITUDE 		: '_vamp_vamp-libxtract_rms_amplitude_rms_amplitude',
	AnnotationTypes.CHRONOGRAM 			: '_vamp_qm-vamp-plugins_qm-chromagram_chromagram',
	AnnotationTypes.SPECTRAL_CENTROID 	: '_vamp_vamp-libxtract_spectral_centroid_spectral_centroid',
	AnnotationTypes.CONSTANT_Q_SPECT 	: '_vamp_qm-vamp-plugins_qm-constantq_constantq',
}

def configuration_file_name_for_annotation_type(annotation_type):
	return str(annotation_type).lower() + '.n3'

def annotation_type_for_annotation_file_name(annotation_file_name):
	annotation_file_name = os.path.splitext(annotation_file_name)[0]
	for annotation_type, suffix in AnnotationFileSuffixes.items():
		if annotation_file_name.endswith(suffix):
			return annotation_type
	return None

