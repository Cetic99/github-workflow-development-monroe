/* eslint-disable react/prop-types */
import styled from '@emotion/styled'
import BackgroundImage from '@ui/assets/images/retail/idle-background.png'
import { useNavigate } from 'react-router-dom'

const Container = styled.div`
  width: 50.438rem;
  height: 20rem;

  position: relative;
  display: flex;
  align-items: flex-end;
  padding: 1.5rem;

  background: url(${BackgroundImage});
  background-size: cover;
  object-fit: fill;

  border-radius: 1rem;

  h2 {
    position: relative;
    z-index: 2;
    font-size: 4.375rem;
    font-weight: 700;
    line-height: 4.375rem;
    color: white;
  }

  & .fade {
    position: absolute;
    bottom: 0;
    left: 0;
    height: 10rem;
    width: 100%;
    background: linear-gradient(180deg, rgba(0, 0, 0, 0) 0%, rgba(0, 0, 0, 0.85) 100%);
    border-radius: 1rem;
  }
`

const CollectionCard = ({ title, to }) => {
  const navigate = useNavigate()

  const handleNavigate = () => {
    navigate(to)
  }

  return (
    <Container onClick={handleNavigate}>
      <h2>{title}</h2>
      <div className="fade"></div>
    </Container>
  )
}

export default CollectionCard
