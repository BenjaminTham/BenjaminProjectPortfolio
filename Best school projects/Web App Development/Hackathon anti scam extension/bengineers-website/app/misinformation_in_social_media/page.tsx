import React from "react";
import './page.css'

export default function MisinformationInSocialMedia() {
  const posts = [
    {
      topicTags: ["Weather", "Health", "Fear"],
      title: "Singapore’s deadliest disease kills 100 within 5 days.",
      link: "https://Boilder-Cooped-News-Singapore-Deadliest-Disease",
      postedOn: "03/10/2024",
      badges: ["ARTIFICIAL INTELLIGENCE", "FAKE"],
      badgesStyles: ["bg-purple-500", "bg-red-500"],
      userTags: ["Highly Biased", "Untrustable"],
      notes: 32,
    },
    {
      topicTags: ["Weather", "Money", "Fear"],
      title:
        "This world is heading towards an unstoppable catastrophe, and no one is talking about it!! Scientists are predicting extreme weather events, food shortages and total economic collapse by end of THIS YEAR!\n\nBig corporations and governments know the truth, and they are hiding it from us. Let’s act together and fight back.\n\nClick on the link below to join hands.\nhttp://f3@oe640.com/21312345323/",
      postedOn: "14/11/2024",
      badges: ["OPINION", "SCAM", "FAKE"],
      badgesStyles: ["bg-blue-500", "bg-orange-500", "bg-red-500"],
      userTags: ["Highly Biased", "Hoax"],
      notes: 1,
    },
    {
      topicTags: ["Education"],
      title:
        "New PSLE syllabus involving more ICT subjects added to government primary schools. Read more at: http://readINews.com",
      postedOn: "",
      badges: [],
      badgesStyles: [],
      userTags: [],
      notes: 0,
    },
  ];

  return (
    <div className="min-h-screen bg-gray-100 p-4">
      <header className="mb-4">
        <h1 className="text-2xl font-bold">BenjiShield</h1>
        <div className="flex gap-4 mt-2">
          <input
            type="text"
            placeholder="Search Topics"
            className="border border-gray-300 rounded-md px-4 py-2 w-1/3"
          />
          <select className="border border-gray-300 rounded-md px-4 py-2">
            <option>Source</option>
          </select>
           
          <input type="date" className="border border-gray-300 rounded-md px-4 py-2" />
        </div>
      </header>
      <main>
        <h2 className="text-xl font-semibold mb-4">Trending</h2>
        {posts.map((post, index) => (
          <div
            key={index}
            className="bg-white p-4 rounded-lg shadow mb-4 border border-gray-200"
          >
            <div className="flex flex-wrap gap-2 mb-2">
              Topics:
              {post.topicTags.map((tag, idx) => (
                <span
                  key={idx}
                  className="bg-gray-200 text-gray-800 px-2 py-1 rounded-full text-sm"
                >
                  {tag}
                </span>
              ))}
            </div>
            <h3 className="font-bold text-lg mb-2">{post.title}</h3>
            {post.link && (
              <a
                href={post.link}
                className="text-blue-500 underline block mb-2"
                target="_blank"
                rel="noopener noreferrer"
              >
                {post.link}
              </a>
            )}
            {post.postedOn && (
              <p className="text-sm text-gray-500">Posted on: {post.postedOn}</p>
            )}
            <div className="flex flex-wrap gap-2 mt-2">
              {post.badges.map((badge, idx) => (
                <span
                  key={idx}
                  className={`text-white px-3 py-1 text-sm rounded-full ${post.badgesStyles[idx]}`}
                >
                  {badge}
                </span>
              ))}
            </div>
            {post.userTags.length > 0 && (
              <div className="mt-2">
                <strong>User Tags:</strong>
                <div className="flex flex-wrap gap-2 mt-1">
                  {post.userTags.map((tag, idx) => (
                    <span
                      key={idx}
                      className="bg-gray-200 text-gray-800 px-2 py-1 rounded-full text-sm"
                    >
                      {tag}
                    </span>
                  ))}
                </div>
              </div>
            )}
            <p className="text-sm text-gray-500 mt-2">
              <i>{post.notes} Notes</i>
            </p>
          </div>
        ))}
      </main>
    </div>
  );
}
