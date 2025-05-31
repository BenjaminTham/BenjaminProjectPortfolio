package sentimentswordcloud;

import org.apache.hadoop.io.IntWritable;
import org.apache.hadoop.io.Text;
import org.apache.hadoop.mapreduce.Partitioner;

public class SentimentWordCloudPartitioner extends Partitioner<Text, IntWritable> {
    @Override
    public int getPartition(Text key, IntWritable value, int numPartitions) {
        // Key format is "word \t sentiment"
        String[] parts = key.toString().split("\t"); // Split word and sentiment
        if (parts.length < 2) {
            return 0; // Send to 0 by default
        }

        // Trim and lowercase sentiment
        String sentiment = parts[1].trim().toLowerCase();

        // With 2 reducers: partition 0 for "positive", partition 1 for any other (i.e., "negative")
        if (sentiment.equals("positive")) {
            return 0; // Return partition 0 (sent to reducer 0 for positive sentiments)
        } else {
            return 1; // Return partition 1 (sent to reducer 1 for negative sentiments)
        }
    }
}
