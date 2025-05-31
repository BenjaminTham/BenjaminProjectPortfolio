package sentimentswordcloud;

import java.io.IOException;
import org.apache.hadoop.io.LongWritable;
import org.apache.hadoop.io.Text;
import org.apache.hadoop.mapreduce.Mapper;

public class ValidationMapper extends Mapper<LongWritable, Text, LongWritable, Text> {

    @Override
    protected void map(LongWritable key, Text value, Context context)
            throws IOException, InterruptedException {
        if (isValid(value.toString())) {
            // If row is Valid, output the key-value pair (key: offset, value: row data)
            context.write(key, value); // this is passed to the next mapper in a chain map
        }
    }

    // Method to check if there are 10 fields in each row
    private boolean isValid(String line) {
        // Expecting 10 fields as per the CSV schema.
        String[] parts = line.split(",");
        return parts.length == 10;
    }
}

