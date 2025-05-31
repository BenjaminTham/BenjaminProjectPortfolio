package examples;

import java.io.IOException;

import org.apache.hadoop.io.IntWritable;
import org.apache.hadoop.io.Text;
import org.apache.hadoop.mapreduce.Reducer;
import org.jline.terminal.impl.jna.freebsd.CLibrary.termios;

public class SocialReducer extends Reducer<Text, Text, Text, Text> {
	IntWritable totalIW = new IntWritable();

	@Override
	public void reduce(Text key, Iterable<Text> values, Context context) throws IOException, InterruptedException {
		int total = 0;
		for (Text value : values) {
			System.out.println("key: " + key + " " + "value: " + value);

			total++;
		}
		totalIW.set(total);
		context.write(key, new Text(totalIW.toString()));
	}
}