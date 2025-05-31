import React from "react";
import Link from "next/link"; // Import Link for navigation

type TopicSummaryProps = {
  topic: string;
};

const summaries: { [key: string]: { summary: string; name: string } } = {
    "Misinformation in Social Media": {
      summary:
        "The spread of false information on social media platforms often leads to polarization and distrust in credible sources.",
      name: "misinformation_in_social_media",
    },
    "Technology": {
      summary:
        "Singapore has continued to solidify its position as a global technology hub, with 2024 marking a pivotal year for advancements in artificial intelligence (AI), smart cities, and sustainable technology solutions. The government’s continued push for the \"Smart Nation\" initiative has seen breakthroughs in urban planning, autonomous transportation, and digital infrastructure.\n" +
          "\n" +
          "The latest trend is the integration of AI into everyday life, from smart homes and predictive healthcare to autonomous vehicles and AI-powered financial services. The government has partnered with major tech giants to develop AI solutions that not only improve the quality of life for residents but also enhance Singapore’s role as a leader in the global digital economy.",
      name: "Technology in Singapore: A Hub for AI and Sustainability Innovations",
    },
    "PresidentialElection": {
      summary:
        "The recent Presidential Election in Singapore has seen a remarkable shift towards a new era of youthful leadership, with candidates increasingly representing a blend of traditional values and modern progressive ideas." +
          "\n" +
          "Voters have been keen on electing a president who can balance both national unity and adaptability to a rapidly changing global environment, especially in areas like digital transformation and sustainability.\n" +
          "\n" +
          "The 2024 election also saw an increased emphasis on digital campaigns, with candidates leveraging social media and online platforms to engage voters in more dynamic, transparent conversations. Interactive town halls, live-streamed debates, and real-time Q&A sessions via social media have redefined traditional campaign methods, making political discourse more accessible and inclusive.\n" +
          "\n" +
          "The increasing influence of younger, digitally-savvy voters has also reshaped campaign strategies, with issues such as climate change, data privacy, and public health becoming central to the candidates’ platforms. In the end, Singaporeans overwhelmingly voted for a candidate who promised to advocate for youth empowerment, technological innovation, and a more sustainable future.",
      name: "Presidential Election in Singapore: A Shift Towards Youthful Leadership",
    },
    "Weather": {
        summary:
            "The island nation, known for its tropical climate, has been experiencing more frequent heatwaves, with temperatures reaching record highs in the past year. These trends have raised concerns over public health, energy consumption, and urban livability. In response, Singapore has accelerated efforts to adapt to its changing climate, with the government rolling out ambitious initiatives to combat urban heat island effects and manage water resources more efficiently. The 'Cool Singapore' campaign, launched in 2024, focuses on integrating green roofs, trees, and reflective surfaces into urban planning to reduce heat absorption and create cooler public spaces. The nation has also faced unpredictable rainfall patterns, with occasional bouts of intense downpours leading to localized flooding.",
        name: "Weather in Singapore: Rising Temperatures and Resilience in the Face of Climate Change",
    },
  };

const TopicSummary: React.FC<TopicSummaryProps> = ({ topic }) => {
const topicData = summaries[topic] || { summary: "No summary available.", name: "unknown" };
const { summary, name } = topicData;

  return (
    <div
      style={{
        border: "1px solid #ccc",
        borderRadius: "8px",
        padding: "16px",
        margin: "8px 0",
      }}
    >
    {/* Button for navigation */}
        <button
        className="rounded-full
         bg-blue-500 text-white px-4 py-2 hover:bg-blue-600"
        style={{ marginTop: "12px" }}
        >
        {name}
        </button>

      {/* <h2 style={{ marginBottom: "8px", color: "#2c3e50" }}>{topic}</h2> */}
      <p style={{ color: "#7f8c8d", lineHeight: "1.5" }}>{summary}</p>
    </div>
  );
};

export default TopicSummary;
