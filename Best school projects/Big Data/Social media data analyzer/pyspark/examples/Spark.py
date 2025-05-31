# Read the text file
lines = spark.read.text("output/part-r-00000")

# Split lines into parts using tab delimiter
parts = lines.rdd.map(lambda line: line.value.split("\t"))

# Convert key and value to integers
pairs = parts.map(lambda x: (x[0], int(x[1])))

# Sort the pairs by key (numerical order)
sorted_pairs = pairs.sortByKey()

# Collect and print the sorted pairs
for key, value in sorted_pairs.collect():
    print("Key:", key, "Value:", value)



import pandas as pd
from wordcloud import WordCloud, STOPWORDS
import matplotlib.pyplot as plt

# Read CSV (adjust 'sep' if needed)
df = pd.read_csv('output/part-r-00000', sep='\t', header=None, names=['keyword', 'count'])

# Convert all keywords to lowercase
df['keyword'] = df['keyword'].str.lower()

# Build the frequencies dictionary
frequencies = df.set_index('keyword')['count'].to_dict()

# Create the word cloud
wordcloud = WordCloud(width=800, height=400,
                      background_color='white',
                      stopwords=STOPWORDS
                      ).generate_from_frequencies(frequencies)

# Display
plt.figure(figsize=(10, 5))
plt.imshow(wordcloud, interpolation='bilinear')
plt.axis('off')
plt.show()

# # Filter for valid parts
# validParts = parts.filter(lambda line: len(line) >= 22 and line[14] is not None and line[21] is not None)

# # Filter for negative sentiments
# negatives = validParts.filter(lambda tweet: tweet[14] == "negative")

# # Extract tweet texts
# tweetTexts = negatives.map(lambda tweet: tweet[21])

# # Split tweet texts into words
# words = tweetTexts.flatMap(lambda text: text.split(" "))

# # Filter for valid words
# validWords = words.filter(lambda word: word is not None and word != "")

# # Modify and clean the words
# keyValues = validWords.map(lambda word: (word.lower().replace("\"", "").replace("@", "").replace("aa", "americanair").replace("cancelled", "canceled")))

# # Reduce by key to get word counts
# wordCounts = keyValues.countByValue()

# # Swap key-value pairs
# countWords = [(v, k) for k, v in wordCounts.items()]

# # Sort the word counts in descending order
# sortedCounts = sorted(countWords, key=lambda x: x[0], reverse=True)

# # Read most frequent words from a text file
# frequentWords = spark.read.text("mostfrequentwords.txt").rdd.map(lambda line: line.value).collect()

# # Convert the frequent words to a set for efficient filtering
# fwSet = set(frequentWords)

# # Filter out the frequent words
# results = [x for x in sortedCounts if x[1] not in fwSet]

# # Take the top 20 results and print them
# for i, (count, word) in enumerate(results[:20], start=1):
#     print(f"{i}: {word}: {count}")
