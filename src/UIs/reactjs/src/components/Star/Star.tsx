import { StarIcon } from "lucide-react";

import classes from "./Star.module.css";

const Star = props => {
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
        {Array.from({ length: 5 }, (_, index) => (
          <StarIcon key={index} size={15} fill="currentColor" aria-hidden="true" />
        ))}
      </div>
    </div>
  );
};

export default Star;
