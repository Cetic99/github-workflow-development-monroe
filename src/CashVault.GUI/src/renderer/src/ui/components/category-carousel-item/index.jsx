/* eslint-disable react/prop-types */
import styled from '@emotion/styled'
import { useNavigate } from 'react-router-dom'

const CategoryCarouselItemContainer = styled.div`
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  margin-right: 2rem;

  & .category-image {
    background-image: url(${(p) => p.image});
    background-size: cover;
    background-position: center;
    object-fit: fill;
    cursor: pointer;

    border-radius: 100%;
    width: 7.5rem;
    height: 7.5rem;
    margin-bottom: 1.5rem;
  }

  h4 {
    font-size: 1rem;
    font-weight: 600;
    line-height: 1.125rem;
  }
`

const CategoryCarouselItem = ({ title, image, to }) => {
  const navigate = useNavigate()

  const handleNavigate = () => {
    navigate(to)
  }

  return (
    <CategoryCarouselItemContainer image={image} onClick={handleNavigate}>
      <div className="category-image"></div>
      <h4>{title}</h4>
    </CategoryCarouselItemContainer>
  )
}

export default CategoryCarouselItem
