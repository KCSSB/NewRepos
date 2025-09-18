import Navbar from "../Home/components/Navbar/Navbar.jsx";
import "./Board.css";
import "../../fonts/fonts.css";

export default function Board() {
  return (
    <div className="board-container">
      <Navbar />
      <div className="board-main-content"></div>
    </div>
  );
}
