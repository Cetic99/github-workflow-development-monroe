/* eslint-disable react/prop-types */
import styled from '@emotion/styled'
import FeaturedBackground from '@ui/assets/images/retail/featured-background.png'
import { useNavigate } from 'react-router-dom'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  justify-content: flex-end;

  position: relative;
  height: 31.25rem;
  width: 100%;

  outline: 2px solid red;

  background-image: url(${FeaturedBackground});
  background-size: cover;
  background-position: center;
  object-fit: fill;

  border-radius: 1rem;

  /* Clip SVG path */
  clip-path: url(#featuredClip);
  -webkit-clip-path: url(#featuredClip);

  & .content {
    margin: 0 0 3rem 3rem;

    h2 {
      font-size: 4.375rem;
      line-height: 4.375rem;
      color: white;
      font-weight: 700;
      margin-bottom: 0.5rem;
    }

    h3 {
      font-size: 1.5rem;
      line-height: 2rem;
      font-weight: 500;
      color: white;
    }
  }

  & .fade {
    position: absolute;
    z-index: -1;
    bottom: 0;
    height: 50%;
    border-radius: 1rem;
    background: linear-gradient(180deg, rgba(0, 0, 0, 0) 0%, rgba(0, 0, 0, 0.75) 100%);
    width: 100%;
  }
`

const Badge = styled.div`
  position: absolute;
  top: 3rem;
  left: 3rem;
  padding: 0.25rem 0.5rem;
  border-radius: 0.25rem;

  background-color: #2bd6b5;
  span {
    color: #004034;
    font-size: 1.5rem;
    font-weight: 700;
    line-height: 2rem;
  }
`
const FloatingText = styled.div`
  position: absolute;
  right: 0;
  top: 0;
  z-index: 12;

  span {
    font-size: 1.5rem;
    line-height: 2rem;
    color: #004034;
    font-weight: 700;
  }
`

const Wrapper = styled.div`
  position: relative;
`

const FeaturedCard = ({ title, subtitle, badgeText, topRightText, to }) => {
  const navigate = useNavigate()

  const handleNavigate = () => {
    navigate(to)
  }

  return (
    <Wrapper onClick={handleNavigate}>
      <Container>
        <div className="content">
          <h2>{title}</h2>
          <h3>{subtitle}</h3>
        </div>

        <Badge>
          <span>{badgeText}</span>
        </Badge>
        <div className="fade"></div>
      </Container>
      <FloatingText>
        <span>{topRightText}</span>
      </FloatingText>

      <svg width="0" height="0">
        <defs>
          <clipPath id="featuredClip" clipPathUnits="objectBoundingBox">
            <path d="M0.597 0C0.617 0 0.636 0.016 0.65 0.045L0.656 0.057C0.671 0.089 0.692 0.11 0.713 0.11H0.723H0.982C0.99 0.11 0.998 0.124 0.998 0.142V0.968C0.998 0.986 0.99 1 0.982 1H0.017C0.007 1 0 0.986 0 0.968V0.032C0 0.014 0.007 0 0.017 0H0.597Z" />
          </clipPath>
        </defs>
      </svg>
    </Wrapper>
  )
}

export default FeaturedCard
