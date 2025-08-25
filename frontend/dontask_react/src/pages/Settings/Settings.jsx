import Navbar from "../Home/components/Navbar/Navbar.jsx";
import ProfileContainer from "./components/Profile/Profile.jsx";
import Mode from "./components/Mode/Mode.jsx";
import "./Settings.css";
import "../../fonts/fonts.css";

export default function Settings() {
  return (
    <div className="settings-container">
      <Navbar />
      <div className="settings-main-content">
        <ProfileContainer />
        <Mode />
      </div>
    </div>
  );
}
