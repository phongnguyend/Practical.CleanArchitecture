import React from "react";
import Image from "next/image";
import Timer from "../../components/Timer/Timer";

import "./Home.css";

function Home() {
  const date = new Date();

  const nextVersion = "16.0.6";

  return (
    <div className="card">
      <div className="card-header">
        Welcome ClassifiedAds NextJs version: {nextVersion}
        <div style={{ float: "right", color: "green" }}>
          <Timer />
        </div>
      </div>
      <div className="card-body">
        <div className="container-fluid">
          <div className="text-center">
            <Image
              src="next.svg"
              alt="logo"
              className="img-responsive center-block"
              width={200}
              height={200}
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
              Practical.CleanArchitecture
            </a>
          </div>
          <div className="text-center">
            <strong>NextJs v{nextVersion}</strong>
          </div>
          <div className="text-center">{date.toLocaleDateString()}</div>
        </div>
      </div>
    </div>
  );
}

export default Home;
