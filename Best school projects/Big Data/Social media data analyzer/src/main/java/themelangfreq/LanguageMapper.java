package themelangfreq;

import java.io.IOException;
import org.apache.hadoop.io.LongWritable;
import org.apache.hadoop.io.Text;
import org.apache.hadoop.mapreduce.Mapper;

public class LanguageMapper extends Mapper<LongWritable, Text, Text, Text> {
	private Text langCode = new Text();
    private Text langTag = new Text();
    
    @Override
    public void map(LongWritable key, Text value, Context context) 
            throws IOException, InterruptedException {
    	    	
        String line = value.toString().trim();
        
        // Skip empty lines or header
        if (line.isEmpty() || line.startsWith("ISO_code,Language")) {
            return;
        }

        // Split and validate
        String[] parts = line.split(",", -1);
        if (parts.length != 2 || parts[0].isEmpty() || parts[1].isEmpty()) {
            System.err.println("Invalid line format: " + line);
            return;
        }

        // Ensure no nulls
        try {
            langCode.set(parts[0].trim());
            langTag.set("l|" + parts[1].trim());
            context.write(langCode, langTag);
        } catch (Exception e) {
            System.err.println("Failed to process line: " + line);
            e.printStackTrace();
        }
    }
}