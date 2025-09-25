import ProjectDescription from "./ProjectDescription.jsx";
import Gantt_chart from "./Gantt_chart.jsx";
import "./Info.css";

export default function Info() {
  return (
    <div className="info-container">
      <ProjectDescription />
      <Gantt_chart />
    </div>
  );
}
