import styled from '@emotion/styled'
import Button from '@ui/components/button'
import ProductCardCart from '@ui/components/cards/product-card-cart'
import IconBadgeDiscount from '@ui/components/icons/IconBadgeDiscount'
import IconCloseCircle from '@ui/components/icons/IconCloseCircle'
import IconRightArrow from '@ui/components/icons/IconRightArrow'
import RetailContainer from '@ui/components/retail-container'
import { useNavigate } from 'react-router-dom'

const Container = styled.div`
  padding-top: 2rem !important;
  position: relative;
  display: flex;
  flex-direction: column;
  margin-bottom: 1.5rem;
  gap: 0 1.5rem;

  height: 100%;

  & .container-wrapper {
    height: 40vh;
    overflow-y: auto;
  }

  .header {
    display: flex;
    flex-direction: column;
    align-items: center;
    width: 100%;
    h1 {
      font-size: 4rem;
      line-height: 4.5rem;
      font-weight: 700;
      margin-bottom: 1.5rem;
    }
    p {
      font-size: 1.5rem;
      line-height: 2rem;
      font-weight: 400;
    }

    margin-bottom: 2.25rem;
  }

  & .products {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
    margin-bottom: 0.5rem;
  }

  & .apply-code {
    margin-bottom: 1.813rem;
    display: flex;
    justify-content: space-between;
    background-color: white;
    border-radius: 1.5rem;
    padding: 2rem;

    & .section-text {
      display: flex;
      align-items: center;
      gap: 1rem;

      & .text {
        h2 {
          font-size: 1.5rem;
          line-height: 2rem;
          font-weight: 400;
        }

        p {
          font-size: 1.25rem;
          line-height: 1.75rem;
          color: #6a6a6a;
          font-weight: 400;
        }
      }
    }
  }

  & .summary {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 0.5rem;
    background-color: #dae1d6;
    padding: 1rem 1.5rem;
    border-radius: 1rem;

    h3 {
      font-size: 1.5rem;
      font-weight: 400;
      line-height: 2rem;
      color: #6a6a6a;
    }

    h2 {
      font-size: 1.5rem;
      font-weight: 700;
      line-height: 2rem;
      color: black;
    }
  }
`

const Footer = styled.div`
  position: absolute;
  bottom: 0;
  left: 0;
  width: 100%;
  height: 9rem;
  background-color: white;

  & .wrapper {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

  gap: 0.5rem;
  & .wave-svg {
    position: absolute;
    top: -3rem;
    left: 0;
  }

  & .amount {
    padding-bottom: 1rem;

    h2 {
      font-size: 2rem;
      line-height: 2.5rem;
      margin-bottom: 0.75rem;
      font-weight: 600;
    }

    h1 {
      font-size: 4rem;
      line-height: 4.5rem;
      font-weight: 700;
    }
  }

  & .buttons {
    padding-bottom: 1rem;

    display: flex;
    height: fit-content;
    gap: 1.5rem;
  }
`
const cartItems = [
  {
    name: 'Nike Sportswear Chill Knit',
    size: 'M',
    color: 'Green',
    quantity: 1,
    price: 29.99,
    image: 'https://placehold.co/600x400'
  },
  {
    name: 'Nike Sportswear Chill Knit',
    size: 'M',
    color: 'Green',
    quantity: 1,
    price: 29.99,
    image: 'https://placehold.co/600x400'
  }
]

const Wave = () => {
  return (
    <svg
      xmlns="http://www.w3.org/2000/svg"
      fill="none"
      className="wave-svg"
      viewBox="0 0 327.68 54.26190476190476"
      width="327.68"
      height="54.26190476190476"
    >
      <path
        fill="white"
        d="M185.852 0C204.759 2.20537e-05 222.734 8.19802 235.215 22.5059L240.522 28.5986C254.411 44.5152 274.065 55.0234 295.092 55.0234H303V55H1036C1044.84 55 1052 62.1634 1052 71V484C1052 492.837 1044.84 500 1036 500H5C-3.83656 500 -11 492.837 -11 484V16C-11 7.16345 -3.83655 2.09384e-07 5 0H185.852Z"
      />
    </svg>
  )
}

const CartScreen = () => {
  const navigate = useNavigate()

  const handleCheckoutNavigate = () => {
    navigate('/retail/checkout')
  }
  return (
    <RetailContainer>
      <Container>
        <div className="header">
          <h1>Your cart</h1>
          <p>{cartItems.length} items</p>
        </div>

        <div className="container-wrapper">
          <div className="products">
            {cartItems.map((item, index) => (
              <ProductCardCart
                key={index}
                title={item.name}
                image={item.image}
                subtitle={`Size: ${item.size}, Color: ${item.color}`}
                price={item.price}
                quantity={item.quantity}
              />
            ))}
          </div>
          <div className="apply-code">
            <div className="section-text">
              <IconBadgeDiscount color="black" size="l" />
              <div className="text">
                <h2>Promo code</h2>
                <p>Redeem coupons or gift cards</p>
              </div>
            </div>
            <Button color="secondary">Apply promo code</Button>
          </div>

          <div className="summary">
            <h3>Cart total</h3>
            <h2>€71.24</h2>
          </div>
          <div className="summary">
            <h3>Tax</h3>
            <h2>€71.24</h2>
          </div>
          <div className="summary">
            <h3>Promo discount</h3>
            <h2>€71.24</h2>
          </div>
        </div>
      </Container>
      <Footer>
        <Wave color="#FFFFFF" />
        <h2>Subtotal:</h2>
        <div className="wrapper">
          <div className="amount">
            <h1>€94.98</h1>
          </div>
          <div className="buttons">
            <Button
              size="s"
              rounded="s"
              color="white"
              icon={(props) => <IconCloseCircle {...props} />}
            >
              Cancel order
            </Button>
            <Button
              onClick={handleCheckoutNavigate}
              size="s"
              rounded="s"
              icon={(props) => <IconRightArrow {...props} />}
            >
              Proceed to checkout
            </Button>
          </div>
        </div>
      </Footer>
    </RetailContainer>
  )
}

export default CartScreen
