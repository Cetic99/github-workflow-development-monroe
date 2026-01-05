import styled from '@emotion/styled'
import RetailContainer from '@ui/components/retail-container'
import { useParams } from 'react-router-dom'
import IdleBackground from '@ui/assets/images/retail/idle-background.png'
import ProductDetails from '@ui/components/product-details'

const Container = styled.div`
  padding-top: 2rem !important;
  position: relative;
  display: flex;
  margin-bottom: 1.5rem;
  gap: 0 1.5rem;

  height: 100%;

  & .product-info {
    width: 50%;
    & .image-carousel {
      width: 100%;
      height: 39.063rem;
      background-image: url(${IdleBackground});
      background-position: center;
      background-size: cover;
      object-fit: fill;
      border-radius: 1rem;
      margin-bottom: 1.625rem;
    }

    & .category {
      color: var(--secondary-dark);
      text-decoration: underline;
      font-size: 1.5rem;
      line-height: 2rem;
      font-weight: 400;
    }

    h1 {
      font-size: 2.5rem;
      font-weight: 400;
      line-height: 3rem;
      margin-bottom: 0.5rem;
    }

    p {
      font-size: 1.5rem;
      line-height: 2rem;
      font-weight: 400;
      margin-bottom: 1rem;
    }

    & .price {
      font-size: 2rem;
      line-height: 2.5rem;
      font-weight: 700;
    }
  }

  & .seperator {
    width: 1px;
    height: 90vh;
    background-color: rgba(204, 211, 199, 1);
  }
`

const ProductScreen = () => {
  const params = useParams()

  const { title } = params
  return (
    <RetailContainer>
      <Container>
        <div className="product-info">
          <div className="image-carousel"></div>
          <span className="category">T-Shirts</span>
          <h1>{title}</h1>
          <p>
            With a sleek fit and smooth, stretchy jersey fabric, this cropped tee is perfect for
            everyday wear.
          </p>
          <span className="price">€29.99</span>
        </div>
        <div className="seperator"></div>
        <ProductDetails />
      </Container>
    </RetailContainer>
  )
}

export default ProductScreen
