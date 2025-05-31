package sentimentswordcloud;

import java.io.BufferedReader;
import java.io.FileReader;
import java.io.IOException;
import java.util.HashSet;
import java.util.Set;

import org.apache.hadoop.io.IntWritable;
import org.apache.hadoop.io.LongWritable;
import org.apache.hadoop.io.Text;
import org.apache.hadoop.mapreduce.Mapper;

public class SentimentWordCloudMapper extends Mapper<LongWritable, Text, Text, IntWritable> {

    private Set<String> stopWords = new HashSet<>(); // Create hash set for stop words

    @Override
    protected void setup(Mapper<LongWritable, Text, Text, IntWritable>.Context context)
            throws IOException {
        // Read stopwords text file from distributed cache       
        BufferedReader br = new BufferedReader(new FileReader("stopwords.txt"));
        String line = null;
        while (true) {
            line = br.readLine();
            if (line != null) {
                stopWords.add(line.trim().toLowerCase());
            } else {
                break; // finished reading
            }
        }
        br.close();
    }

    @Override
    protected void map(LongWritable key, Text value, Context context)
            throws IOException, InterruptedException {
        // Split the single row into parts
        String[] parts = value.toString().split(","); // split using "," because it is a CSV file

        // Extract relevant fields
        String originalText = parts[1];
        String language = parts[4].trim();
        String sentimentString = parts[7].trim();

        // Filter out non-english rows
        if (!language.equalsIgnoreCase("en")) {
            return;
        }

        double sentimentValue; // Ranges from -1 to 1
        try {
            sentimentValue = Double.parseDouble(sentimentString);
        } catch(NumberFormatException e) {
            // Skip if sentiment is not a valid number
            return;
        }

        // Classify the sentiment (if < 0 it is negative, if > 0 it is positive)
        String sentimentCategory = sentimentValue < 0 ? "negative" : "positive";

        // Seperate the original text into words (split by spaces)
        String[] words = originalText.split("\\s+"); // regex for one or more spaces

        // Loop through words
        for (String word : words) {
            if (word != null && !word.isEmpty()) { // Check if the word is not null or empty

                // Trim and lowercase the word
                word = word.trim().toLowerCase();

                // Remove punctuation (keep only letters)
                word = word.replaceAll("[^a-z]", "");

                // Skip words that are too short (single letters) or unusually long
                if (word.length() < 2 || word.length() > 15) {
                    continue;
                }

                // Skip words that are made up of a single repeated character (e.g. "aaaaaaaaaaa")
                if (word.matches("^(.)\\1+$")) {
                    continue;
                }

                // Check if any word becomes empty after punctuation removal
                if (word.isEmpty()) {
                    continue; // Skip this iteration if the word is empty after cleaning
                }
            
                // Filter out stop words
                if (!stopWords.contains(word)) {

                    // Emit key value pair (key: word \t sentiment , value: 1) e.g (happy \t positive, 1)
                    context.write(new Text(word + "\t" + sentimentCategory), new IntWritable(1));

                }

            }
        }
    }
}

