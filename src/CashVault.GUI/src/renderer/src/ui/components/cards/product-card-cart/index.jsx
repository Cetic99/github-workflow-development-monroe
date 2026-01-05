/* eslint-disable react/prop-types */

import styled from '@emotion/styled'
import { useNavigate } from 'react-router-dom'
import CircleButton from '@ui/components/circle-button'
import IconMinus from '@ui/components/icons/IconMinus'
import IconPlus from '@ui/components/icons/IconPlus'
import { useState } from 'react'
import IconTrashCan from '@ui/components/icons/IconTrashCan'

const Container = styled.div`
  display: flex;
  justify-content: space-between;
  align-items: center;
  background-color: white;
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
    gap: 0 1.5rem;

    & .price-remove {
      display: flex;
      align-items: center;
      gap: 1.5rem;
    }

    h3 {
      color: #020605;
      font-size: 1.5rem;
      line-height: 2rem;
      font-weight: 400;
    }

    & .buttons {
      display: flex;
      align-items: center;
      gap: 0 1.5rem;

      & .button {
        padding: 0;
      }

      span {
        font-size: 1.5rem;
        line-height: 2rem;
        font-weight: 600;
      }
    }
  }
`

const ProductCardCart = ({ title, subtitle, price, discount = 0, to, image, quantity }) => {
  const [itemQuantity, setItemQuantity] = useState(quantity)
  const navigate = useNavigate()

  const handleNavigate = () => {
    navigate(to)
  }

  const handleDecrease = () => {
    if (itemQuantity - 1 < 0 || itemQuantity === 0) {
      return
    }

    setItemQuantity(itemQuantity - 1)
  }

  const handleIncrease = () => {
    setItemQuantity(itemQuantity + 1)
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
        <div className="buttons">
          <CircleButton
            className="button"
            color="transparent"
            size="s"
            onClick={handleDecrease}
            icon={(props) => <IconMinus {...props} />}
          ></CircleButton>
          <span>{itemQuantity}</span>
          <CircleButton
            className="button"
            color="transparent"
            size="s"
            onClick={handleIncrease}
            icon={(props) => <IconPlus {...props} />}
          ></CircleButton>
        </div>
        <div className="price-remove">
          <h3>€ {price}</h3>
          <span onClick={handleNavigate}>
            <IconTrashCan size="m" color="var(--secondary-dark)" />
          </span>
        </div>
      </div>
    </Container>
  )
}

export default ProductCardCart
