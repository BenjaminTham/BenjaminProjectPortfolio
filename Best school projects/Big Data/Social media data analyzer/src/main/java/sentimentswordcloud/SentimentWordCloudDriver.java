package sentimentswordcloud;

import java.io.IOException;
import java.net.URI;
import java.net.URISyntaxException;
import org.apache.hadoop.conf.Configuration;
import org.apache.hadoop.fs.Path;
import org.apache.hadoop.io.IntWritable;
import org.apache.hadoop.io.Text;
import org.apache.hadoop.mapreduce.Job;
import org.apache.hadoop.mapreduce.lib.input.FileInputFormat;
import org.apache.hadoop.mapreduce.lib.output.FileOutputFormat;
import org.apache.hadoop.util.GenericOptionsParser;

public class SentimentWordCloudDriver {
    public static void main(String[] args)
            throws IOException, URISyntaxException, ClassNotFoundException, InterruptedException {
        Configuration conf = new Configuration();
        String[] otherArgs = new GenericOptionsParser(conf, args).getRemainingArgs();

        // Check if have 3 arguments (input path, output path, stopwords file)
        if (otherArgs.length < 3) {
            System.err.println("Usage: SentimentWordCloudDriver <input> <output> <stopwords textfile>");
            System.exit(2);
        }

        // Create job
        Job job = Job.getInstance(conf, "Word Count By Sentiment");

        // Set driver class for the job
        job.setJarByClass(SentimentWordCloudDriver.class);

        // Define input and output path arguments
        Path inPath = new Path(otherArgs[0]); // Input path
        Path outPath = new Path(otherArgs[1]); // Output path

        // Delete output path if it exists
        outPath.getFileSystem(conf).delete(outPath, true);

        // Put this file to distributed cache so we can use it in mapper (stopwords file)
        job.addCacheFile(new URI(otherArgs[2]));

        // Set SentimentWordCloudMapper as the job's mapper
        job.setMapperClass(SentimentWordCloudMapper.class);

        // Set reducer class
        job.setReducerClass(SentimentWordCloudReducer.class);

        // Set output class (key: Text, value: IntWritable)
        job.setOutputKeyClass(Text.class);
        job.setOutputValueClass(IntWritable.class);

        // Set input and output paths for the job
        FileInputFormat.addInputPath(job, inPath);
        FileOutputFormat.setOutputPath(job, outPath);

        // Check if job is completed before exiting program
        boolean status = job.waitForCompletion(true);
        System.exit(status ? 0 : 1);
    }
}

