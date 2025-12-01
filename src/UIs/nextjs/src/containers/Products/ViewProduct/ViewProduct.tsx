"use client";

import { useEffect, useState } from "react";
import Image from "next/image";
import Link from "next/link";
import { useRouter, useParams } from "next/navigation";
import Star from "../../../components/Star/Star";
import axios from "../axios";

interface Product {
  id: string;
  name: string;
  code: string;
  description: string;
  price: number;
  starRating: number;
  imageUrl: string;
}

const ViewProduct = () => {
  const { id } = useParams();
  const router = useRouter();
  const [product, setProduct] = useState<Product | null>(null);
  const fetchProduct = async (id: string) => {
    try {
      const response = await axios.get(id);
      const fetchedProduct = response.data;
      setProduct(fetchedProduct);
    } catch (error) {
      console.log(error);
    }
  };

  useEffect(() => {
    if (id && typeof id === "string") {
      fetchProduct(id);
    }
  }, []);

  const back = () => {
    router.push("/products");
  };

  const page = product ? (
    <div className="card">
      <div className="card-header">{"Product Detail: " + product.name}</div>

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
            <Image
              className="center-block img-responsive"
              alt=""
              style={{ width: "200px", margin: "2px" }}
              src={product.imageUrl || "../next.svg"}
              title={product.name}
              width={50}
              height={50}
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
        <Link className="btn btn-primary" href={"/products/edit/" + product.id}>
          Edit
        </Link>
      </div>
    </div>
  ) : null;
  return page;
};

export default ViewProduct;
