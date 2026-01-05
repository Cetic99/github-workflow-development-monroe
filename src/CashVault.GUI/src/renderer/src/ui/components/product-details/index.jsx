import styled from '@emotion/styled'
import { useState } from 'react'
import IconMinus from '../icons/IconMinus'
import IconPlus from '../icons/IconPlus'
import CircleButton from '../circle-button'
import IconExpand from '../icons/IconExpand'
import IconBorderlessCheck from '../icons/IconBorderlessCheck'
import Button from '../button'
import IconShoppingCart from '../icons/IconShoppingCart'

const Container = styled.div`
  width: 50%;
  & .quantity {
    width: 100%;
    margin-bottom: 2rem;
    h2 {
      font-size: 2rem;
      line-height: 2.5rem;
      font-weight: 600;
      margin-bottom: 0.5rem;
    }

    p {
      font-size: 2rem;
      line-height: 2.5rem;
      font-weight: 400;
      margin-bottom: 1.5rem;
    }

    & .buttons {
      display: flex;
      align-items: center;
      gap: 0 2rem;
      & .button {
        padding: 0;
      }
      span {
        font-size: 2rem;
        line-height: 2.5rem;
        font-weight: 600;
      }
    }
  }

  & .separator {
    width: 100%;
    height: 1px;
    background-color: rgba(204, 211, 199, 1);
  }

  & .size {
    margin-top: 3rem;
    margin-bottom: 2rem;

    & .buttons {
      display: flex;
      gap: 1rem;
      flex-wrap: wrap;

      & .button {
        padding: 0;
      }
    }

    & .heading {
      display: flex;
      justify-content: space-between;
      align-items: center;
      h2 {
        font-size: 2rem;
        line-height: 2.5rem;
        font-weight: 600;
        margin-bottom: 0.5rem;
      }
      & .icon {
        display: flex;
        gap: 0 0.5rem;
        font-size: 1.5rem;
        line-height: 2rem;
        color: var(--secondary-dark);
        span {
          text-decoration: underline;
        }
      }
    }
    p {
      font-size: 2rem;
      line-height: 2.5rem;
      font-weight: 400;
      margin-bottom: 1.5rem;
    }
  }

  & .color {
    margin-top: 2rem;
    margin-bottom: 2rem;

    & .colors {
      display: flex;
      gap: 1rem;
      flex-wrap: wrap;
    }

    h2 {
      font-size: 2rem;
      line-height: 2.5rem;
      font-weight: 600;
      margin-bottom: 0.5rem;
    }

    p {
      font-size: 2rem;
      line-height: 2.5rem;
      font-weight: 400;
      margin-bottom: 1.5rem;
    }
  }

  & .subtotal {
    margin-top: 2rem;
    text-align: center;

    h2 {
      font-size: 2.5rem;
      line-height: 3rem;
      font-weight: 700;
      margin-bottom: 2rem;
    }

    h3 {
      font-size: 2rem;
      line-height: 2.5rem;
      font-weight: 400;
    }

    & .button {
      width: 100%;
    }
  }
`

const ColorOption = styled.div`
  cursor: pointer;
  background-color: ${(p) => p.color};
  width: 4rem;
  height: 4rem;
  border-radius: 100%;

  display: flex;
  justify-content: center;
  align-items: center;
  padding: 4px;

  box-sizing: border-box;

  & .active-wrapper {
    position: absolute;
    border: ${(p) => (p.chosen ? '4px solid var(--secondary-dark)' : 'none')};
    border-radius: 100%;
    width: 5rem;
    height: 5rem;
  }
`

const InnerWrapper = styled.div`
  width: 100%;
  height: 100%;
  border-radius: 100%;
  display: flex;
  justify-content: center;
  align-items: center;
  box-sizing: border-box;
  background-color: ${(p) => p.color};
`

const availableSizes = ['M', 'S', 'L', 'XL', 'XXL', '3XL']
const availableColors = ['#15c552', '#B6BBAA', '#FE9E59', '#FF5151']

const ProductDetails = () => {
  const [quantity, setQuantity] = useState(0)
  const [selectedSize, setSelectedSize] = useState('')
  const [selectedColor, setSelectedColor] = useState('')

  const handleDecrease = () => {
    if (quantity - 1 < 0 || quantity === 0) {
      return
    }

    setQuantity(quantity - 1)
  }

  const handleIncrease = () => {
    setQuantity(quantity + 1)
  }

  const handleSelectedSize = (value) => {
    if (value === selectedSize) {
      setSelectedSize('')
      return
    }

    setSelectedSize(value)
  }

  const handleSelectedColor = (value) => {
    if (value === selectedColor) {
      setSelectedColor('')
      return
    }

    setSelectedColor(value)
  }

  return (
    <Container>
      <div className="quantity">
        <h2>Quantity</h2>
        <p>How much do you want?</p>
        <div className="buttons">
          <CircleButton
            className="button"
            color="secondary"
            size="s"
            onClick={handleDecrease}
            icon={(props) => <IconMinus {...props} />}
          ></CircleButton>
          <span>{quantity}</span>
          <CircleButton
            className="button"
            color="secondary"
            size="s"
            onClick={handleIncrease}
            icon={(props) => <IconPlus {...props} />}
          ></CircleButton>
        </div>
      </div>
      <div className="separator"></div>
      <div className="size">
        <div className="heading">
          <h2>Size</h2>
          <div className="icon">
            <IconExpand color="var(--secondary-dark)" />
            <span>Size Guide</span>
          </div>
        </div>
        <p>Pick your size</p>
        <div className="buttons">
          {availableSizes.map((size, index) => (
            <CircleButton
              className="button"
              color={size === selectedSize ? 'secondary' : 'white'}
              key={index}
              textInner={size}
              onClick={() => handleSelectedSize(size)}
            />
          ))}
        </div>
      </div>
      <div className="separator"></div>
      <div className="color">
        <h2>Color</h2>
        <p>Pick the desired color.</p>
        <div className="colors">
          {availableColors.map((color, index) => (
            <ColorOption
              color={color}
              key={index}
              chosen={color === selectedColor}
              onClick={() => handleSelectedColor(color)}
            >
              <div className="active-wrapper"></div>
              <InnerWrapper color={color}>
                {color === selectedColor && <IconBorderlessCheck color="white" />}
              </InnerWrapper>
            </ColorOption>
          ))}
        </div>
      </div>
      <div className="separator"></div>
      <div className="subtotal">
        <h3>Subtotal</h3>
        <h2>€59.98</h2>
        <Button className="button" color="dark" icon={(props) => <IconShoppingCart {...props} />}>
          Add to cart
        </Button>
      </div>
    </Container>
  )
}

export default ProductDetails
