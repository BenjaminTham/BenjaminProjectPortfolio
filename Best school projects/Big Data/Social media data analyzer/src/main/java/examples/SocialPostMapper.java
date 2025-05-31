package examples;

import java.io.IOException;

import org.apache.hadoop.io.LongWritable;
import org.apache.hadoop.io.Text;
import org.apache.hadoop.mapreduce.Mapper;

public class SocialPostMapper extends Mapper<LongWritable, Text, Text, Text> {

    /*
     * Mapper (domain, +1)
     */

    @Override
    public void map(LongWritable key, Text value, Context context) throws IOException, InterruptedException {
        // regex to split on commas that are not inside quotes.
        String[] parts = value.toString().split(",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");

        // If this is the header line or there aren't enough columns, skip the record.
        if (parts.length < 10) {
            return;
        }

        // region --- url extraction ---
        // Extract the URL (assuming it's the third column) and clean it.
        String url = parts[2].trim();
        if (url.startsWith("\"") && url.endsWith("\"")) {
            url = url.substring(1, url.length() - 1);
        }
        // regex for url cleaning to remove the dynamic post id's
        String domain = url.replaceAll("https?://([^/]+).*", "$1");
        // endregion

        if (domain != null) {
            context.write(new Text(domain), new Text("Domain"));
            System.out.println("Domain In SocialPostMapper: " + domain);

        }

    }

}
