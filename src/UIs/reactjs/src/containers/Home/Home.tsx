import React from "react";
import ActionIcon from "../../components/ActionIcon/ActionIcon";
import logo from "../../logo.svg";
import Timer from "../../components/Timer/Timer";

import "./Home.css";

function Home() {
  const date = new Date();
  return (
    <div className="card">
      <div className="card-header">
        <ActionIcon action="home" />
        Welcome ClassifiedAds React version: {React.version}
        <div style={{ float: "right", color: "green" }}>
          <Timer />
        </div>
      </div>
      <div className="card-body">
        <div className="container-fluid">
          <div className="text-center">
            <img
              src={logo}
              className="img-responsive center-block"
              style={{ maxHeight: "300px", paddingBottom: "10px" }}
            />
          </div>

          <div className="text-center">Developed by:</div>
          <div className="text-center">
            <h3>Phong Nguyen</h3>
          </div>

          <div className="text-center">@phongnguyend</div>
          <div className="text-center">
            <a href="https://github.com/phongnguyend/Practical.CleanArchitecture">
              <ActionIcon action="external" /> Practical.CleanArchitecture
            </a>
          </div>
          <div className="text-center">
            <strong>react v{React.version}</strong>
          </div>
          <div className="text-center">{date.toLocaleDateString()}</div>
        </div>
      </div>
    </div>
  );
}

export default Home;
