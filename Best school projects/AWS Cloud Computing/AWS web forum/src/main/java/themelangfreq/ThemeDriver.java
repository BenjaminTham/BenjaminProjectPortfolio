package themelangfreq;

import java.io.IOException;
import java.net.URISyntaxException;
import org.apache.hadoop.conf.Configuration;
import org.apache.hadoop.fs.Path;
import org.apache.hadoop.io.Text;
import org.apache.hadoop.mapreduce.Job;
import org.apache.hadoop.mapreduce.lib.input.TextInputFormat;
import org.apache.hadoop.mapreduce.lib.input.MultipleInputs;
import org.apache.hadoop.mapreduce.lib.output.FileOutputFormat;
import org.apache.hadoop.util.GenericOptionsParser;

public class ThemeDriver {
	
	public static void main(String[] args) throws IOException, URISyntaxException, ClassNotFoundException, InterruptedException {
        Configuration conf = new Configuration();
        String[] otherArgs = new GenericOptionsParser(conf, args).getRemainingArgs();

        if (otherArgs.length < 2) {
            System.err.println("Usage: ThemeDriver <data> <output> <ISO>");
            System.exit(2);
        }

        Job job = Job.getInstance(conf, "Theme Frequency by Language");
        job.setJarByClass(ThemeDriver.class);
        
        MultipleInputs.addInputPath(job, new Path(otherArgs[2]), TextInputFormat.class, LanguageMapper.class);
        MultipleInputs.addInputPath(job, new Path(otherArgs[0]), TextInputFormat.class, ThemeMapper.class);
        new Path(args[1]).getFileSystem(conf).delete(new Path(args[1]), true);
        
        job.setMapOutputKeyClass(Text.class);
        job.setMapOutputValueClass(Text.class);
        
        job.setReducerClass(ThemeReducer.class);
        job.setOutputKeyClass(Text.class);
        job.setOutputValueClass(Text.class);

        FileOutputFormat.setOutputPath(job, new Path(args[1]));

        boolean status = job.waitForCompletion(true);
        System.exit(status ? 0 : 1);
    }
}
