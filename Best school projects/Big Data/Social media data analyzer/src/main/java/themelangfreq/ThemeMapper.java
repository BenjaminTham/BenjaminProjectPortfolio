package themelangfreq;

import java.io.IOException;
import java.util.Arrays;
import java.util.HashSet;
import java.util.Set;
import org.apache.hadoop.io.LongWritable;
import org.apache.hadoop.io.Text;
import org.apache.hadoop.mapreduce.Mapper;

public class ThemeMapper extends Mapper<LongWritable, Text, Text, Text> {

    // Define the valid themes as a static set for fast lookup.
    private static final Set<String> VALID_THEMES = new HashSet<>(Arrays.asList(
        "Economy", "Technology", "Investing", "Business", "Cryptocurrency",
        "Social", "Politics", "Finance", "Entertainment", "Health",
        "Law", "Sports", "Science", "Environment", "People"
    ));

    @Override
    public void map(LongWritable key, Text value, Context context) throws IOException, InterruptedException {
    	String[] parts = value.toString().split(",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
        if (parts.length >= 10) {
            String language = parts[4].trim().replaceAll("^\"|\"$", "");
            String theme = parts[parts.length - 5].trim().replaceAll("^\"|\"$", "");
            
            // Check if theme is one of the valid themes
            if (!VALID_THEMES.contains(theme)) {
                return;
            }
            
            Text i_theme = new Text("c|" + theme);
            context.write(new Text(language), i_theme);
        }
    }
}