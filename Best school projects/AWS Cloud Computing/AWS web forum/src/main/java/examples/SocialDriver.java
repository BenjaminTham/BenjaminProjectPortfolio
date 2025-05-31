
package examples;

import java.io.IOException;

import org.apache.hadoop.conf.Configuration;
import org.apache.hadoop.fs.Path;
import org.apache.hadoop.io.IntWritable;
import org.apache.hadoop.io.Text;
import org.apache.hadoop.mapreduce.Job;
import org.apache.hadoop.mapreduce.lib.input.FileInputFormat;
import org.apache.hadoop.mapreduce.lib.output.FileOutputFormat;
import org.apache.hadoop.util.GenericOptionsParser;

public class SocialDriver {

	public static void main(String[] args) throws IOException, ClassNotFoundException, InterruptedException {
		Configuration conf = new Configuration();
		String[] otherArgs = new GenericOptionsParser(conf, args).getRemainingArgs();
		Job job = Job.getInstance(conf, "Social Media Analysis");
		Path outPath = new Path(otherArgs[1]);
		outPath.getFileSystem(conf).delete(outPath, true);

		job.setJarByClass(SocialDriver.class);
		job.setMapperClass(SocialMapper.class);
		// job.setCombinerClass(StatisticsReducer.class);
		job.setReducerClass(SocialReducer.class);

		job.setOutputKeyClass(Text.class);
		job.setOutputValueClass(Text.class);

		FileInputFormat.addInputPath(job, new Path(otherArgs[0]));
		FileOutputFormat.setOutputPath(job, outPath);

		System.exit(job.waitForCompletion(true) ? 0 : 1);

	}

}