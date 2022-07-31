import { useEffect } from "react";
import { NavLink, useParams, useNavigate } from "react-router-dom";
import { useDispatch, useSelector } from "react-redux";

import logo from "../../../logo.svg";
import Star from "../../../components/Star/Star";
import * as actions from "../actions";

const ViewProduct = () => {

  const { id } = useParams();
  const navigate = useNavigate();
  const { product } = useSelector((state: any) => state.product);
  const dispatch = useDispatch();
  const fetchProduct = id => dispatch(actions.fetchProduct(id));

  useEffect(() => {
    if (id) {
      fetchProduct(id);
    }
  }, []);

  const back = () => {
    navigate("/products");
  };

  const page = product ? (
    <div className="card">
      <div className="card-header">
        {"Product Detail: " + product.name}
      </div>

      <div className="card-body">
        <div className="row">
          <div className="col-md-8">
            <div className="row">
              <div className="col-md-4">Name:</div>
              <div className="col-md-8">{product.name}</div>
            </div>
            <div className="row">
              <div className="col-md-4">Code:</div>
              <div className="col-md-8">{product.code}</div>
            </div>
            <div className="row">
              <div className="col-md-4">Description:</div>
              <div className="col-md-8">{product.description}</div>
            </div>
            <div className="row">
              <div className="col-md-4">Price:</div>
              <div className="col-md-8">{product.price || 5}</div>
            </div>
            <div className="row">
              <div className="col-md-4">5 Star Rating:</div>
              <div className="col-md-8">
                <Star rating={product.starRating || 4} />
              </div>
            </div>
          </div>

          <div className="col-md-4">
            <img
              className="center-block img-responsive"
              style={{ width: "200px", margin: "2px" }}
              src={product.imageUrl || logo}
              title={product.name}
            />
          </div>
        </div>
      </div>

      <div className="card-footer">
        <button
          className="btn btn-outline-secondary"
          onClick={back}
          style={{ width: "80px" }}
        >
          <i className="fa fa-chevron-left"></i> Back
        </button>
        &nbsp;
        <NavLink
          className="btn btn-primary"
          to={"/products/edit/" + product.id}
        >
          Edit
        </NavLink>
      </div>
    </div>
  ) : null;
  return page;
}

export default ViewProduct;
