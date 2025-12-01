import React from "react";

import classes from "./Star.module.css";

interface StarProps {
  rating: number;
  ratingClicked?: (message: string) => void;
}

const Star = (props: StarProps) => {
  const width = (props.rating * 75) / 5;
  return (
    <div
      className={classes.Star}
      style={{ width: width }}
      title="rating"
      onClick={() =>
        props.ratingClicked
          ? props.ratingClicked(`The rating ${props.rating} was clicked!`)
          : null
      }
    >
      <div style={{ width: "75px" }}>
        <span className="fa fa-star"></span>
        <span className="fa fa-star"></span>
        <span className="fa fa-star"></span>
        <span className="fa fa-star"></span>
        <span className="fa fa-star"></span>
      </div>
    </div>
  );
};

export default Star;
