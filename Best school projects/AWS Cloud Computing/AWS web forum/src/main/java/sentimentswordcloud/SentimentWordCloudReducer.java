package sentimentswordcloud;

import java.io.IOException;
import org.apache.hadoop.io.IntWritable;
import org.apache.hadoop.io.Text;
import org.apache.hadoop.mapreduce.Reducer;

public class SentimentWordCloudReducer extends Reducer<Text, IntWritable, Text, IntWritable> {

    @Override
    protected void reduce(Text key, Iterable<IntWritable> values, Context context)
            throws IOException, InterruptedException {

        int sum = 0;

        // Sum all counts for the given composite key (key: word \t sentiment)
        for (IntWritable val : values) {
            sum += val.get();
        }
        // Write output in the format: "word \t sentiment \t count"
        context.write(key, new IntWritable(sum)); // e.g (happy \t positive \t 35)
    }
}

