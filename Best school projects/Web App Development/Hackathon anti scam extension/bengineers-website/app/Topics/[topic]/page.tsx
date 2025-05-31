'use client'; // Mark the component as a client component

import React, {useEffect, useState} from "react";
import './page.css';
import { initializeApp } from "firebase/app";
import { getFirestore, collection, getDocs, query, where } from "firebase/firestore";
import { useParams } from "next/navigation";
import { AreaChart, CartesianGrid, XAxis, Area } from "recharts";
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faCommentAlt } from '@fortawesome/free-solid-svg-icons';
import {
    ChartConfig,
  ChartContainer,
  ChartTooltip,
  ChartTooltipContent,
} from "@/components/ui/chart";
import Link from "next/link";
import TopicSummary from "@/lib/Components/AISummaryComponent";

// Firebase configuration
const firebaseConfig = {
    apiKey: "AIzaSyDyUws3vjsFtKJrCxS7xNOxLeNqg2df-pI",
    authDomain: "bengineering-hackrift.firebaseapp.com",
    projectId: "bengineering-hackrift",
    storageBucket: "bengineering-hackrift.appspot.com",
    messagingSenderId: "467110689511",
    appId: "1:467110689511:web:b48a00564cb7b5f36e0042",
};

const app = initializeApp(firebaseConfig);
const db = getFirestore(app);

type Post = {
    title: string;
    postid?: string;
    reliabilitytag?: string[];
    approvednotes?: string[];
    timestamp?: string;
    sourcetag?: string[];
    unapprovednotes?: number[];
    topictag?: string[];
    tagCount?: number;
    notesCount?: number;
    content?: string;
};

async function getPosts(tag: string): Promise<Post[]> {
    const postsCol = collection(db, 'posts');
    const q = query(postsCol, where("topictag", "array-contains", tag));
    const querySnapshot = await getDocs(q);
    return querySnapshot.docs.map(doc => {
        const data = doc.data();
        console.log(data)
        return {
            title: data.title || '',
            postid: data.postid || '',
            reliabilitytag: Array.isArray(data.reliabilitytag) ? data.reliabilitytag : [],
            approvednotes: Array.isArray(data.approvednotes) ? data.approvednotes : [],
            timestamp: data.timestamp || '',
            sourcetag: Array.isArray(data.sourcetag) ? data.sourcetag : [],
            unapprovednotes: Array.isArray(data.unapprovednotes) ? data.unapprovednotes : [],
            topictag: Array.isArray(data.topictag) ? data.topictag : [],
            tagCount: data.reliabilitytag?.length + data.sourcetag?.length || 0,
            notesCount: data.approvednotes?.length + data.unapprovednotes?.length || 0,
            content: data.content || "",
        };
    });
}

export default function Topics() {
    const [posts, setPosts] = useState<Post[]>([]);
    const [loading, setLoading] = useState(true);
    const [chartData, setChartData] = useState<{ timestamp: string; tagCount: number }[]>([]);
    const params = useParams();
    const topictag = params.topic;
    const [searchTerm, setSearchTerm] = useState("");
    const [selectedSource, setSelectedSource] = useState("");
    const [selectedReliability, setSelectedReliability] = useState("");
    const [selectedDate, setSelectedDate] = useState(""); // New state for date filtering

    const [visibleNotes, setVisibleNotes] = useState<{ [key: string]: boolean }>({});

    const toggleNotesVisibility = (postId: string | undefined, notesCount: number) => {
      if (postId && notesCount > 0) {
          setVisibleNotes((prevState) => ({
              ...prevState,
              [postId]: !prevState[postId] // Toggle visibility for the specific post
          }));
      }
  };

    useEffect(() => {
        async function fetchPosts() {
            try {
                const data = await getPosts(topictag);
                setPosts(data);

                // Prepare chart data
                const groupedData: { [key: string]: number } = {};
                data.forEach(post => {
                    if (post.timestamp) {
                        const dateKey = new Date(post.timestamp).toLocaleDateString();
                        groupedData[dateKey] = (groupedData[dateKey] || 0) + (post.tagCount || 0);
                        // console.log(groupedData);
                    }
                });

        // Format grouped data for the chart
        const formattedData = Object.entries(groupedData).map(([timestamp, tagCount]) => ({
          timestamp,
          tagCount,
        }))
        .sort((a, b) => new Date(a.timestamp).getTime() - new Date(b.timestamp).getTime()); // Sort by date
        console.log(formattedData);
        setChartData(formattedData);
      } catch (error) {
        console.error('Error fetching posts:', error);
      } finally {
        setLoading(false);
      }
    }
    fetchPosts();
  }, [topictag]);

  

    const chartConfig = {
        desktop: {
            label: "Desktop",
            color: "hsl(var(--chart-1))",
        },
    } satisfies ChartConfig

    const filteredPosts = posts.filter((post) => {
        const matchesSearchTerm = searchTerm
            ? post.topictag?.some((tag) =>
                tag.toLowerCase().includes(searchTerm.toLowerCase())
            )
            : true;

        const matchesSource = selectedSource
            ? post.sourcetag?.includes(selectedSource)
            : true;

        const matchesReliability = selectedReliability
            ? post.reliabilitytag?.includes(selectedReliability)
            : true;

        // Date filtering logic
        let matchesDate = true;
        if (selectedDate && post.timestamp) {
            // Convert both to Date objects
            const postDate = new Date(post.timestamp);
            const filterDate = new Date(selectedDate);
            // Example: Only show posts posted on or after the selected date
            matchesDate = postDate >= filterDate;
        }

        return matchesSearchTerm && matchesSource && matchesReliability && matchesDate;
    });

    const getReliabilityTagColor = (tag: string) => {
        switch (tag) {
            case 'Reliable':
                return 'bg-green-500'; // Green
            case 'Mostly-Reliable':
                return 'bg-yellow-500'; // Yellow
            case 'Mixed':
                return 'bg-blue-500'; // Blue
            case 'Likely-False':
                return 'bg-orange-500'; // Orange
            case 'False':
                return 'bg-red-500'; // Red
            default:
                return 'bg-gray-500'; // Default color
        }
    };

    const getSourceTagColor = (tag: string) => {
        switch (tag) {
            case 'AI':
                return 'bg-pink-500'; // Pink
            case 'Opinion':
                return 'bg-blue-800'; // Dark Blue
            case 'News':
                return 'bg-gray-800'; // Dark Gray
            case 'Advertising':
                return 'bg-purple-500'; // Purple
            default:
                return 'bg-gray-500'; // Default color
        }
    };

  return (
    <div className="min-h-screen bg-gray-100 p-4" style={{ padding: 0 }}>

    <header className="mb-4" style={{ display: "flex", justifyContent: "space-around", height: "5rem" }}>
        <Link href={'/'}>
            <h1 className="text-3xl " style={{ fontFamily: 'Roboto, sans-serif', color: "#022B3A80" }}>BenjiShield</h1>
        </Link>
            <div className="flex gap-4 mt-2">
            <input
                type="text"
                placeholder="Search Topics"
                className="border border-gray-300 rounded-md px-4 py-2 w-1/3"
                style={{ width: "120%" }}
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
            />
            <select
                className="border border-gray-300 rounded-md px-4 py-2"
                value={selectedSource}
                onChange={(e) => setSelectedSource(e.target.value)}
            >
                <option value="">All Sources</option>
                <option value="AI">AI</option>
                <option value="Opinion">Opinion</option>
                <option value="News">News</option>
                <option value="Advertising">Advertising</option>
            </select>
            <select
                className="border border-gray-300 rounded-md px-4 py-2"
                value={selectedReliability}
                onChange={(e) => setSelectedReliability(e.target.value)}
            >
                <option value="">All Reliability</option>
                <option value="Reliable">Reliable</option>
                <option value="Mostly-Reliable">Mostly-Reliable</option>
                <option value="Mixed">Mixed</option>
                <option value="Likely-False">Likely-False</option>
                <option value="False">False</option>
            </select>

            <input
                type="date"
                style={{ display: "inline-block", height: "50px", width: "120%" }}
                value={selectedDate}
                onChange={(e) => setSelectedDate(e.target.value)}
            />
        </div>
    </header>
      <main style={{ padding: 16 }}>
          <h2> Summary of trends </h2>
          <TopicSummary topic={topictag} />

        <h2 className="font-semibold mb-4">Topic: {topictag}</h2>
        {loading ? (
          <p>Loading posts...</p>
        ) : (
          <>
            {chartData.length > 0 ? (
              <ChartContainer config={chartConfig}>
                <AreaChart
                  width={250}
                  height={100}
                  data={chartData}
                  margin={{ top: 10, right: 30, left: 0, bottom: 0 }}
                >
                  <CartesianGrid strokeDasharray="3 3" />
                  <XAxis dataKey="timestamp" tickLine={false} />
                  <ChartTooltip
                    cursor={false}
                    content={<ChartTooltipContent indicator="line" />}
                  />
                  <Area
                    type="monotone"
                    dataKey="tagCount"
                    stroke="#8884d8"
                    fill="#8884d8"
                    fillOpacity={0.3}
                  />
                </AreaChart>
              </ChartContainer>
            ) : (
              <p>No data available for the timeline.</p>
            )}

            {filteredPosts.length > 0 ? (
              filteredPosts.map((post, index) => (
<div
                            key={index}
                            className="bg-white p-4 rounded-lg shadow mb-4 border border-gray-200"
                        >
                            <div className="flex flex-wrap gap-2 mb-2">
                                Topics:
                                {post.topictag?.map((tag, idx) => (
                                    <Link key={idx} href={post.topictag ? `/Topics/${post.topictag}` : '#'}>
                                        <button
                                            className="topic-buttons rounded-md bg-gray-200 text-black px-4 py-2 hover:bg-gray-300"
                                            disabled={!post.topictag}
                                        >
                                            {post.topictag || 'No Tag'}
                                        </button>
                                    </Link>
                                ))}
                            </div>
                           
                            {post.postid && (
                                <a
                                    href={post.postid}
                                    className="text-blue-500 underline block mb-2"
                                    target="_blank"
                                    rel="noopener noreferrer"
                                > <h3 className="font-bold text-lg mb-2">
                                    {post.content}
                                    </h3>
                                </a>
                            )}
                            {post.timestamp && (
                                <p className="text-sm text-gray-500">Posted on: {post.timestamp}</p>
                            )}
                            <div className="tags-container">
                                <div className="flex flex-wrap gap-2 mt-2">
                                    {post.sourcetag?.map((badge, idx) => (
                                        <span
                                            key={idx}
                                            className={`tags text-white px-3 py-1 text-sm rounded-md ${getSourceTagColor(badge)}`}
                                        >
                                            {badge}
                                        </span>
                                    ))}
                                </div>
                                <div className="flex flex-wrap gap-2 mt-2">
                                    {post.reliabilitytag?.map((badge, idx) => (
                                        <span
                                            key={idx}
                                            className={`tags text-white px-3 py-1 text-sm rounded-md ${getReliabilityTagColor(badge)}`}
                                        >
                                            {badge}
                                        </span>
                                    ))}
                                </div>
                            </div>
                            <p className="notes-container text-sm text-gray-500 mt-2">
                                <button
                                    onClick={() => toggleNotesVisibility(post.postid, post.notesCount || 0)}
                                    className="flex items-center space-x-2 text-blue-500 hover:text-blue-700"
                                >
                                    <FontAwesomeIcon icon={faCommentAlt} />
                                    <span>{post.notesCount || 0} Notes</span>
                                </button>
                            </p>

                            {visibleNotes[post.postid || ''] && (
                                <div className="notes-details mt-4">
                                    <hr />
                                    <span className="title">Approved Note:</span>
                                    {post.approvednotes}
                                    <br />
                                    <span className="title">Unapproved Note(s):</span>
                                    {post.unapprovednotes?.map((note, idx) => (
                                        <p key={idx}>
                                            {note}
                                        </p>
                                    ))}
                                </div>
                            )}
                        </div>
              ))
            ) : (
              <p>No posts available.</p>
            )}
          </>
        )}
      </main>
    </div>
  );
}