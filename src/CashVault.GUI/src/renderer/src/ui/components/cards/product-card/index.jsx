/* eslint-disable react/prop-types */
import styled from '@emotion/styled'
import { useTranslation } from '@src/app/domain/administration/stores'
import BackgroundImage from '@ui/assets/images/retail/idle-background.png'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  justify-content: flex-end;

  position: relative;
  background-image: url(${BackgroundImage});
  background-size: cover;
  background-position: center;
  object-fit: fill;

  border-radius: 1rem;

  height: 20rem;
  width: 17.813rem;

  & .fade {
    position: absolute;
    z-index: 0;
    bottom: 0;
    height: 50%;
    border-radius: 1rem;
    background: linear-gradient(
      180deg,
      rgba(${(p) => hexToRgb(p.fadeColor)}, 0) 0%,
      rgba(${(p) => hexToRgb(p.fadeColor)}, 0.85) 100%
    );
    width: 100%;
  }

  & .discount-badge {
    position: absolute;
    top: 1.5rem;
    left: 1.5rem;
    background-color: #2bd6b5;
    padding: 0.25rem 0.5rem;
    border-radius: 0.25rem;

    span {
      font-size: 1.5rem;
      line-height: 2rem;
      font-weight: 700;
      color: #004034;
    }
  }

  & .content {
    position: relative;
    z-index: 2;

    padding: 0 1.5rem 1.5rem 1.5rem;
  }

  h4 {
    font-size: 1.5rem;
    line-height: 2rem;
    color: white;
    font-weight: 500;
  }

  h5 {
    margin-top: 0.5rem;
    font-size: 1.5rem;
    line-height: 2rem;
    color: ${(p) => (p.discount != 0 ? '#2BD6B5' : 'white')};
    font-weight: 700;
  }

  & .price {
    display: flex;
    gap: 1rem;
    align-items: flex-end;

    & .discount-from {
      font-size: 1rem;
      line-height: 1.5rem;
      color: white;
      text-decoration: line-through;
    }
  }
`
const hexToRgb = (hex) => {
  hex = hex.replace(/^#/, '')

  if (hex.length === 3) {
    hex = hex
      .split('')
      .map((c) => c + c)
      .join('')
  }

  const bigint = parseInt(hex, 16)
  const r = (bigint >> 16) & 255
  const g = (bigint >> 8) & 255
  const b = bigint & 255

  return `${r},${g},${b}`
}

const ProductCard = ({
  title,
  subtitle,
  discount = 0,
  fadeColor = '#000000',
  onClick = () => {}
}) => {
  const { t } = useTranslation()

  const handleOnClick = () => {
    onClick()
  }

  return (
    <Container fadeColor={fadeColor} discount={discount} onClick={handleOnClick}>
      <div className="content">
        <h4>{t(title)}</h4>
        <div className="price">
          <h5>€ {discount === 0 ? subtitle : (subtitle - subtitle * discount).toPrecision(4)}</h5>
          {discount !== 0 && <h5 className="discount-from">€ {subtitle}</h5>}
        </div>
      </div>
      {discount !== 0 && (
        <div className="discount-badge">
          <span>- {discount * 100} %</span>
        </div>
      )}
      <div className="fade"></div>
    </Container>
  )
}

export default ProductCard
