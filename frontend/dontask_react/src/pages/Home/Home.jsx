import Navbar from "./components/Navbar/Navbar";
import Background from "./components/Background/Background";
import Project_list from "./components/Project_list/Project_list";
import "./Home.css";
import "../../fonts/fonts.css";

export default function Home() {
  return (
    <div className="home-container">
      <Navbar />
      <div className="home-main-content">
        <Background />
        <Project_list />
      </div>
    </div>
  );
}
