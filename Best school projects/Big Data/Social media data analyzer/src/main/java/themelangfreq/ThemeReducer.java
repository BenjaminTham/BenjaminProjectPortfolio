package themelangfreq;

import java.io.IOException;
import java.util.HashMap;
import java.util.Map;

import org.apache.hadoop.io.Text;
import org.apache.hadoop.mapreduce.Reducer;

public class ThemeReducer extends Reducer<Text, Text, Text, Text> {
    private Map<String, Integer> themeCounts = new HashMap<>();
    private String languageName = null;

    @Override
    public void reduce(Text key, Iterable<Text> values, Context context) 
            throws IOException, InterruptedException {
        
        themeCounts.clear();  // Reset counts for each language
        languageName = null;  // Reset language name
        
        for (Text value : values) {
            String valStr = value.toString();
            
            // Check if it's a language name (from LanguageMapper)
            if (valStr.startsWith("l|")) {
                languageName = valStr.substring(2);  // Extract "English" from "l|English"
            } 
            // Check if it's a theme count (from ThemeMapper)
            else if (valStr.startsWith("c|")) {
                String theme = valStr.substring(2);  // Extract "Technology" from "c|Technology"
                themeCounts.put(theme, themeCounts.getOrDefault(theme, 0) + 1);
            }
        }
        
        // If we found a language name, emit counts
        if (languageName != null) {
            for (Map.Entry<String, Integer> entry : themeCounts.entrySet()) {
                String outputKey = languageName + " - " + entry.getKey();  // e.g., "English - Technology"
                context.write(new Text(outputKey), new Text(entry.getValue().toString()));
                System.out.println(new Text(outputKey + " > " + entry.getValue().toString()));
            }
        }
    }
}