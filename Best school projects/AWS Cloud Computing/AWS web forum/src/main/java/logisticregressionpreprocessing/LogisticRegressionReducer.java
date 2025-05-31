package logisticregressionpreprocessing;

import java.io.IOException;

import org.apache.hadoop.io.IntWritable;
import org.apache.hadoop.io.Text;
import org.apache.hadoop.mapreduce.Reducer;

public class LogisticRegressionReducer extends Reducer<Text, Text, Text, Text> {
    IntWritable totalIW = new IntWritable();

    @Override
    public void reduce(Text key, Iterable<Text> values, Context context) throws IOException, InterruptedException {
        StringBuilder sb = new StringBuilder();
        for (Text value : values) {
            if (sb.length() > 0) {
                sb.append(","); // or ", " if you prefer a space
            }
            sb.append(value.toString());
        }
        // This will print something like: "sentiment word1:val1 word2:val2, sentiment
        // word1:val1 word2:val2, ..."
        System.out.println("Reducer output - key: " + key + " value: " + sb.toString());
        context.write(key, new Text(sb.toString()));
    }
}
