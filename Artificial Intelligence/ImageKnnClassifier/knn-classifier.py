import os
import math
import numpy as np
from PIL import Image
from collections import Counter


def get_normalized_histogram(image_path):
	image = Image.open(image_path)
	np_histogram = np.array(image.histogram())
	normalized_histogram = np_histogram / np.linalg.norm(np_histogram)
	return normalized_histogram.tolist()


def get_histograms(category):
	histograms = []
	image_filenames = os.listdir(category)
	for image_filename in image_filenames:
		if image_filename.endswith('jpeg') or image_filename.endswith('jpg'):
			image_filepath = os.path.join(category, image_filename)

			histogram = get_normalized_histogram(image_filepath)

			histograms.append(histogram)

	return histograms


def similarity(histogram_a, histogram_b):
    sum_dist = 0
    for i in range(len(histogram_a)):
        sum_dist += ((histogram_a[i] - histogram_b[i]) * (histogram_a[i] - histogram_b[i]))

    return math.sqrt(sum_dist)


def classify(input_path):
    beach_histograms = get_histograms('beaches')
    mountain_histograms = get_histograms('mountains')
    city_histograms = get_histograms('cities')

    input_histogram = get_normalized_histogram(input_path)

    similarities = []
    for histogram in beach_histograms:
    	similarities.append(('beach', similarity(histogram, input_histogram)))

    for histogram in mountain_histograms:
    	similarities.append(('mountain', similarity(histogram, input_histogram)))

    for histogram in city_histograms:
    	similarities.append(('city', similarity(histogram, input_histogram)))

    sorted_tuples = sorted(similarities, key=lambda tup: tup[1])

    classification = Counter(zip(*sorted_tuples[:3])[0]).most_common(1)[0][0]

    print 'The image was cassified as ' + classification + '.'


def main():
    classify('input-beach.jpg')
    classify('input-city.jpg')


if __name__ == '__main__':
    main()
