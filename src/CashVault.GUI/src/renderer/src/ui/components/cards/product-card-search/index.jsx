/* eslint-disable react/prop-types */

import styled from '@emotion/styled'
import { useNavigate } from 'react-router-dom'
import IconRightHalfArrow from '@ui/components/icons/IconRightHalfArrow'

const Container = styled.div`
  display: flex;
  justify-content: space-between;
  align-items: center;
  background-color: #dae1d6;
  padding: 2rem;
  border-radius: 1.5rem;

  & .product-image {
    width: 5rem;
    height: 5rem;
    border-radius: 0.5rem;
    background-image: ${(p) => `url(${p.image})`};
    background-position: center;
    background-size: cover;
    object-fit: fill;
  }

  & .left-side {
    display: flex;
    align-items: center;
    gap: 1.5rem;

    h3 {
      color: #020605;
      font-size: 1.5rem;
      line-height: 2rem;
      font-weight: 500;
    }
    h4 {
      color: var(--text-default);
      font-weight: 400;
      font-size: 1.25rem;
      line-height: 1.75rem;
    }
  }

  & .price {
    display: flex;
    align-items: center;
    gap: 1.5rem;

    h3 {
      color: #020605;
      font-size: 1.5rem;
      line-height: 2rem;
      font-weight: 700;
    }

    & .discount-badge {
      padding: 0.25rem 0.5rem;
      background-color: #2bd6b5;
      color: #004034;
      font-size: 1.5rem;
      font-weight: 700;
      line-height: 2rem;
      border-radius: 0.25rem;
      margin-right: 1rem;
    }
  }
`

const ProductCardSearch = ({ title, subtitle, price, discount = 0, to, image }) => {
  const navigate = useNavigate()

  const handleNavigate = () => {
    navigate(to)
  }

  return (
    <Container image={image} discount={discount}>
      <div className="left-side">
        <div className="product-image"></div>
        <div className="content">
          <h3>{title}</h3>
          <h4>{subtitle}</h4>
        </div>
      </div>
      <div className="price">
        {discount != 0 && <span className="discount-badge">- {discount * 100}%</span>}
        <h3>€ {price}</h3>
        <span onClick={handleNavigate}>
          <IconRightHalfArrow size="l" color="var(--secondary-dark)" />
        </span>
      </div>
    </Container>
  )
}

export default ProductCardSearch
